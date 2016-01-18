#region Copyright
//=======================================================================================
// Microsoft Azure Customer Advisory Team  
//
// This sample is supplemental to the technical guidance published on the community
// blog at http://blogs.msdn.com/b/paolos/. 
// 
// Author: Paolo Salvatori
//=======================================================================================
// Copyright © 2015 Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER 
// EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF 
// MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE. YOU BEAR THE RISK OF USING IT.
//=======================================================================================
#endregion

#region Using Directives

using System;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.AzureCat.Samples.PayloadEntities;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace Microsoft.AzureCat.Samples.AlertClient
{
    public partial class MainForm : Form
    {
        #region Private Constants
        //***************************
        // Formats
        //***************************
        private const string DateFormat = "<{0,2:00}:{1,2:00}:{2,2:00}> {3}";
        private const string ExceptionFormat = "Exception: {0}";
        private const string InnerExceptionFormat = "InnerException: {0}";
        private const string LogFileNameFormat = "AlertClientLog-{0}.txt";

        //***************************
        // Constants
        //***************************
        private const string SaveAsTitle = "Save Log As";
        private const string SaveAsExtension = "txt";
        private const string SaveAsFilter = "Text Documents (*.txt)|*.txt";
        private const string Start = "Start";
        private const string Stop = "Stop";

        //***************************
        // Columns
        //***************************
        private const string DeviceId = "DeviceId";
        private const string DeviceName = "Name";
        private const string Value = "Value";
        private const string Model = "Model";
        private const string Manufacturer = "Manufacturer";
        private const string Type = "Type";
        private const string City = "City";
        private const string Country = "Country";

        //***************************
        // Configuration Parameters
        //***************************
        private const string DefaultEventHubName = "DeviceDemoOutputHub";
        private const string DefaultConsumerGroupName = "AlertClient";
        private const string StorageAccountConnectionStringParameter = "storageAccountConnectionString";
        private const string ServiceBusConnectionStringParameter = "serviceBusConnectionString";
        private const string EventHubParameter = "eventHub";
        private const string ConsumerGroupParameter = "consumerGroup";

        //***************************
        // Messages
        //***************************
        private const string StorageAccountConnectionStringCannotBeNull = "The Storage Account connection string cannot be null.";
        private const string ServiceBusConnectionStringCannotBeNull = "The Service Bus connection string cannot be null.";
        private const string EventHubNameCannotBeNull = "The Event Hub name cannot be null.";
        private const string ConsumerGroupCannotBeNull = "The Consumer Group name cannot be null.";
        private const string RegisteringEventProcessor = "Registering Event Processor [EventProcessor]... ";
        private const string EventProcessorRegistered = "Event Processor [EventProcessor] successfully registered. ";
        #endregion

        #region Public Constructor
        /// <summary>
        /// Initializes a new instance of the MainForm class.
        /// </summary>
        public MainForm()
        {
            InitializeComponent();
            ConfigureComponent();
            ReadConfiguration();
        }
        #endregion

        #region Private Fields
        private readonly SortableBindingList<Alert> alertBindingList = new SortableBindingList<Alert> { AllowNew = false, AllowEdit = false, AllowRemove = false };
        private EventProcessorHost eventProcessorHost;
        #endregion

        #region Public Methods

        public void ConfigureComponent()
        {
            txtStorageAccountConnectionString.AutoSize = false;
            txtStorageAccountConnectionString.Size = new Size(txtStorageAccountConnectionString.Size.Width, 24);
            txtServiceBusConnectionString.AutoSize = false;
            txtServiceBusConnectionString.Size = new Size(txtServiceBusConnectionString.Size.Width, 24);
            cboEventHub.AutoSize = false;
            cboEventHub.Size = new Size(cboEventHub.Size.Width, 24);
            txtConsumerGroup.AutoSize = false;
            txtConsumerGroup.Size = new Size(txtConsumerGroup.Size.Width, 24);
            // Set Grid style
            alertDataGridView.EnableHeadersVisualStyles = false;
            alertDataGridView.AutoGenerateColumns = false;
            alertDataGridView.AutoSize = true;

            // Create the DeviceId column
            var textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = DeviceId,
                Name = DeviceId
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Create the Name column
            textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = DeviceName,
                Name = DeviceName
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Create the Value column
            textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = Value,
                Name = Value
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Create the Model column
            textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = Model,
                Name = Model
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Create the Type column
            textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = Type,
                Name = Type
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Create the Manufacturer column
            textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = Manufacturer,
                Name = Manufacturer
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Create the City column
            textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = City,
                Name = City
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Create the Country column
            textBoxColumn = new DataGridViewTextBoxColumn
            {
                DataPropertyName = Country,
                Name = Country
            };
            alertDataGridView.Columns.Add(textBoxColumn);

            // Set the selection background color for all the cells.
            alertDataGridView.DefaultCellStyle.SelectionBackColor = Color.FromArgb(92, 125, 150);
            alertDataGridView.DefaultCellStyle.SelectionForeColor = SystemColors.Window;

            // Set RowHeadersDefaultCellStyle.SelectionBackColor so that its default 
            // value won't override DataGridView.DefaultCellStyle.SelectionBackColor.
            alertDataGridView.RowHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(153, 180, 209);

            // Set the background color for all rows and for alternating rows.  
            // The value for alternating rows overrides the value for all rows. 
            alertDataGridView.RowsDefaultCellStyle.BackColor = SystemColors.Window;
            alertDataGridView.RowsDefaultCellStyle.ForeColor = SystemColors.ControlText;
            //alertDataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            //alertDataGridView.AlternatingRowsDefaultCellStyle.ForeColor = SystemColors.ControlText;

            // Set the row and column header styles.
            alertDataGridView.RowHeadersDefaultCellStyle.BackColor = Color.FromArgb(215, 228, 242);
            alertDataGridView.RowHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;
            alertDataGridView.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(215, 228, 242);
            alertDataGridView.ColumnHeadersDefaultCellStyle.ForeColor = SystemColors.ControlText;

            // Set DataGridView DataSource
            alertBindingSource.DataSource = alertBindingList;
            alertDataGridView.DataSource = alertBindingSource;

            dataGridView_Resize(alertDataGridView, null);
        }

        public void HandleException(Exception ex)
        {
            if (string.IsNullOrEmpty(ex?.Message))
            {
                return;
            }
            WriteToLog(string.Format(CultureInfo.CurrentCulture, ExceptionFormat, ex.Message));
            if (!string.IsNullOrEmpty(ex.InnerException?.Message))
            {
                WriteToLog(string.Format(CultureInfo.CurrentCulture, InnerExceptionFormat, ex.InnerException.Message));
            }
        }
        #endregion

        #region Private Methods
        public static bool IsJson(string item)
        {
            if (item == null)
            {
                throw new ArgumentException("The item argument cannot be null.");
            }
            try
            {
                var obj = JToken.Parse(item);
                return obj != null;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string IndentJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }
            dynamic parsedJson = JsonConvert.DeserializeObject(json);
            return JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
        }

        private void ReadConfiguration()
        {
            try
            {
                txtStorageAccountConnectionString.Text = ConfigurationManager.AppSettings[StorageAccountConnectionStringParameter];
                txtServiceBusConnectionString.Text = ConfigurationManager.AppSettings[ServiceBusConnectionStringParameter];
                var eventHubValue = ConfigurationManager.AppSettings[EventHubParameter] ?? DefaultEventHubName;
                var eventHubs = eventHubValue.Split(',', ';');
                foreach (var eventHub in eventHubs)
                {
                    cboEventHub.Items.Add(eventHub);
                }
                cboEventHub.SelectedIndex = 0;
                txtConsumerGroup.Text = ConfigurationManager.AppSettings[ConsumerGroupParameter] ?? DefaultConsumerGroupName;
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void WriteToLog(string message)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string>(InternalWriteToLog), message);
            }
            else
            {
                InternalWriteToLog(message);
            }
        }

        private void InternalWriteToLog(string message)
        {
            lock (this)
            {
                if (string.IsNullOrEmpty(message))
                {
                    return;
                }
                var lines = message.Split('\n');
                var now = DateTime.Now;
                var space = new string(' ', 19);

                for (var i = 0; i < lines.Length; i++)
                {
                    if (i == 0)
                    {
                        var line = string.Format(DateFormat,
                                                 now.Hour,
                                                 now.Minute,
                                                 now.Second,
                                                 lines[i]);
                        lstLog.Items.Add(line);
                    }
                    else
                    {
                        lstLog.Items.Add(space + lines[i]);
                    }
                }
                lstLog.SelectedIndex = lstLog.Items.Count - 1;
                lstLog.SelectedIndex = -1;
            }
        }

        #endregion

        #region Event Handlers

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void clearLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        /// <summary>
        /// Saves the log to a text file
        /// </summary>
        /// <param name="sender">MainForm object</param>
        /// <param name="e">System.EventArgs parameter</param>
        private void saveLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (lstLog.Items.Count <= 0)
                {
                    return;
                }
                saveFileDialog.Title = SaveAsTitle;
                saveFileDialog.DefaultExt = SaveAsExtension;
                saveFileDialog.Filter = SaveAsFilter;
                saveFileDialog.FileName = string.Format(LogFileNameFormat, DateTime.Now.ToString(CultureInfo.CurrentUICulture).Replace('/', '-').Replace(':', '-'));
                if (saveFileDialog.ShowDialog() != DialogResult.OK || 
                    string.IsNullOrEmpty(saveFileDialog.FileName))
                {
                    return;
                }
                using (var writer = new StreamWriter(saveFileDialog.FileName))
                {
                    foreach (var t in lstLog.Items)
                    {
                        writer.WriteLine(t as string);
                    }
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private void logWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            splitContainer.Panel2Collapsed = !((ToolStripMenuItem)sender).Checked;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new AboutForm();
            form.ShowDialog();
        }

        private void lstLog_Leave(object sender, EventArgs e)
        {
            lstLog.SelectedIndex = -1;
        }

        private void button_MouseEnter(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                control.ForeColor = Color.White;
            }
        }

        private void button_MouseLeave(object sender, EventArgs e)
        {
            var control = sender as Control;
            if (control != null)
            {
                control.ForeColor = SystemColors.ControlText;
            }
        }
        
        // ReSharper disable once FunctionComplexityOverflow
        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            var width = (mainHeaderPanel.Size.Width - 48) / 2;

            cboEventHub.Size = new Size(width, cboEventHub.Size.Height);
            txtConsumerGroup.Size = new Size(width, txtConsumerGroup.Size.Height);
            txtConsumerGroup.Location = new Point(32 + width, txtConsumerGroup.Location.Y);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            txtServiceBusConnectionString.SelectionLength = 0;
        }

        // ReSharper disable once FunctionComplexityOverflow
        private async void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                Cursor.Current = Cursors.WaitCursor;

                if (string.Compare(btnStart.Text, Start, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    // Validate parameters
                    if (!ValidateParameters())
                    {
                        return;
                    }
                    btnStart.Enabled = false;

                    var eventHubClient = EventHubClient.CreateFromConnectionString(txtServiceBusConnectionString.Text,
                        cboEventHub.Text);

                    // Get the default Consumer Group
                    eventProcessorHost = new EventProcessorHost(Guid.NewGuid().ToString(),
                        eventHubClient.Path.ToLower(),
                        txtConsumerGroup.Text.ToLower(),
                        txtServiceBusConnectionString.Text,
                        txtStorageAccountConnectionString.Text)
                    {
                        PartitionManagerOptions = new PartitionManagerOptions
                        {
                            AcquireInterval = TimeSpan.FromSeconds(10), // Default is 10 seconds
                            RenewInterval = TimeSpan.FromSeconds(10), // Default is 10 seconds
                            LeaseInterval = TimeSpan.FromSeconds(30) // Default value is 30 seconds
                        }
                    };
                    WriteToLog(RegisteringEventProcessor);
                    var eventProcessorOptions = new EventProcessorOptions
                    {
                        InvokeProcessorAfterReceiveTimeout = true,
                        MaxBatchSize = 100,
                        PrefetchCount = 100,
                        ReceiveTimeOut = TimeSpan.FromSeconds(30),
                    };
                    eventProcessorOptions.ExceptionReceived += EventProcessorOptions_ExceptionReceived;
                    var eventProcessorFactoryConfiguration = new EventProcessorFactoryConfiguration
                    {
                        TrackEvent = a => Invoke(new Action<Alert>(i => alertBindingList.Add(i)), a),
                        WriteToLog = m => Invoke(new Action<string>(WriteToLog), m)
                    };
                    await
                        eventProcessorHost.RegisterEventProcessorFactoryAsync(
                            new EventProcessorFactory<EventProcessor>(eventProcessorFactoryConfiguration),
                            eventProcessorOptions);
                    WriteToLog(EventProcessorRegistered);

                    // Change button text
                    btnStart.Text = Stop;
                }
                else
                {
                    // Stop Event Processor
                    if (eventProcessorHost != null)
                    {
                        await eventProcessorHost.UnregisterEventProcessorAsync();
                    }

                    // Change button text
                    btnStart.Text = Start;
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
                btnStart.Enabled = true;
            }
        }

        private void EventProcessorOptions_ExceptionReceived(object sender, ExceptionReceivedEventArgs e)
        {
            if (e?.Exception != null)
            {
                WriteToLog(e.Exception.Message);
            }
        }

        private bool ValidateParameters()
        {
            if (string.IsNullOrWhiteSpace(txtServiceBusConnectionString.Text))
            {
                WriteToLog(StorageAccountConnectionStringCannotBeNull);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtServiceBusConnectionString.Text))
            {
                WriteToLog(ServiceBusConnectionStringCannotBeNull);
                return false;
            }
            if (string.IsNullOrWhiteSpace(cboEventHub.Text))
            {
                WriteToLog(EventHubNameCannotBeNull);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtConsumerGroup.Text))
            {
                WriteToLog(ConsumerGroupCannotBeNull);
                return false;
            }
            return true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
            alertBindingList.Clear();
        }

        private void logTabControl_DrawItem(object sender, DrawItemEventArgs e)
        {
            DrawTabControlTabs(logTabControl, e, null);
        }

        private void DrawTabControlTabs(TabControl tabControl, DrawItemEventArgs e, ImageList images)
        {
            // Get the bounding end of tab strip rectangles.
            var tabstripEndRect = tabControl.GetTabRect(tabControl.TabPages.Count - 1);
            var tabstripEndRectF = new RectangleF(tabstripEndRect.X + tabstripEndRect.Width, tabstripEndRect.Y - 5,
            tabControl.Width - (tabstripEndRect.X + tabstripEndRect.Width), tabstripEndRect.Height + 5);
            var leftVerticalLineRect = new RectangleF(2, tabstripEndRect.Y + tabstripEndRect.Height + 2, 2, tabControl.TabPages[tabControl.SelectedIndex].Height + 2);
            var rightVerticalLineRect = new RectangleF(tabControl.TabPages[tabControl.SelectedIndex].Width + 4, tabstripEndRect.Y + tabstripEndRect.Height + 2, 2, tabControl.TabPages[tabControl.SelectedIndex].Height + 2);
            var bottomHorizontalLineRect = new RectangleF(2, tabstripEndRect.Y + tabstripEndRect.Height + tabControl.TabPages[tabControl.SelectedIndex].Height + 2, tabControl.TabPages[tabControl.SelectedIndex].Width + 4, 2);
            RectangleF leftVerticalBarNearFirstTab = new Rectangle(0, 0, 2, tabstripEndRect.Height + 2);

            // First, do the end of the tab strip.
            // If we have an image use it.
            if (tabControl.Parent.BackgroundImage != null)
            {
                var src = new RectangleF(tabstripEndRectF.X + tabControl.Left, tabstripEndRectF.Y + tabControl.Top, tabstripEndRectF.Width, tabstripEndRectF.Height);
                e.Graphics.DrawImage(tabControl.Parent.BackgroundImage, tabstripEndRectF, src, GraphicsUnit.Pixel);
            }
            // If we have no image, use the background color.
            else
            {
                using (Brush backBrush = new SolidBrush(tabControl.Parent.BackColor))
                {
                    e.Graphics.FillRectangle(backBrush, tabstripEndRectF);
                    e.Graphics.FillRectangle(backBrush, leftVerticalLineRect);
                    e.Graphics.FillRectangle(backBrush, rightVerticalLineRect);
                    e.Graphics.FillRectangle(backBrush, bottomHorizontalLineRect);
                    if (tabControl.SelectedIndex != 0)
                    {
                        e.Graphics.FillRectangle(backBrush, leftVerticalBarNearFirstTab);
                    }
                }
            }

            // Set up the page and the various pieces.
            var page = tabControl.TabPages[e.Index];
            using (var backBrush = new SolidBrush(page.BackColor))
            {
                using (var foreBrush = new SolidBrush(page.ForeColor))
                {
                    var tabName = page.Text;

                    // Set up the offset for an icon, the bounding rectangle and image size and then fill the background.
                    var iconOffset = 0;
                    Rectangle tabBackgroundRect;

                    if (e.Index == tabControl.SelectedIndex)
                    {
                        tabBackgroundRect = e.Bounds;
                        e.Graphics.FillRectangle(backBrush, tabBackgroundRect);
                    }
                    else
                    {
                        tabBackgroundRect = new Rectangle(e.Bounds.X, e.Bounds.Y - 2, e.Bounds.Width,
                                                          e.Bounds.Height + 4);
                        e.Graphics.FillRectangle(backBrush, tabBackgroundRect);
                        var rect = new Rectangle(e.Bounds.X - 2, e.Bounds.Y - 2, 1, 2);
                        e.Graphics.FillRectangle(backBrush, rect);
                        rect = new Rectangle(e.Bounds.X - 1, e.Bounds.Y - 2, 1, 2);
                        e.Graphics.FillRectangle(backBrush, rect);
                        rect = new Rectangle(e.Bounds.X + e.Bounds.Width, e.Bounds.Y - 2, 1, 2);
                        e.Graphics.FillRectangle(backBrush, rect);
                        rect = new Rectangle(e.Bounds.X + e.Bounds.Width + 1, e.Bounds.Y - 2, 1, 2);
                        e.Graphics.FillRectangle(backBrush, rect);
                    }

                    // If we have images, process them.
                    if (images != null)
                    {
                        // Get sice and image.
                        var size = images.ImageSize;
                        Image icon = null;
                        if (page.ImageIndex > -1)
                            icon = images.Images[page.ImageIndex];
                        else if (page.ImageKey != "")
                            icon = images.Images[page.ImageKey];

                        // If there is an image, use it.
                        if (icon != null)
                        {
                            var startPoint =
                                new Point(tabBackgroundRect.X + 2 + ((tabBackgroundRect.Height - size.Height) / 2),
                                          tabBackgroundRect.Y + 2 + ((tabBackgroundRect.Height - size.Height) / 2));
                            e.Graphics.DrawImage(icon, new Rectangle(startPoint, size));
                            iconOffset = size.Width + 4;
                        }
                    }

                    // Draw out the label.
                    var labelRect = new Rectangle(tabBackgroundRect.X + iconOffset, tabBackgroundRect.Y + 5,
                                                  tabBackgroundRect.Width - iconOffset, tabBackgroundRect.Height - 3);
                    using (var sf = new StringFormat { Alignment = StringAlignment.Center })
                    {
                        e.Graphics.DrawString(tabName, new Font(e.Font.FontFamily, 8.25F, e.Font.Style), foreBrush, labelRect, sf);
                    }
                }
            }
        }

        private void dataGridView_Resize(object sender, EventArgs e)
        {
            CalculateLastColumnWidth(sender);
        }

        private void dataGridView_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            CalculateLastColumnWidth(sender);
            var dataGridView = sender as DataGridView;
            if (dataGridView == null)
            {
                return;
            }
            dataGridView.FirstDisplayedScrollingRowIndex = dataGridView.RowCount - 1;
        }

        private void dataGridView_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            CalculateLastColumnWidth(sender);
        }

        private void CalculateLastColumnWidth(object sender)
        {
            var dataGridView = sender as DataGridView;
            if (dataGridView == null)
            {
                return;
            }
            try
            {
                dataGridView.SuspendLayout();
                var width = dataGridView.Width - dataGridView.RowHeadersWidth;
                var verticalScrollbar = dataGridView.Controls.OfType<VScrollBar>().First();
                if (verticalScrollbar.Visible)
                {
                    width -= verticalScrollbar.Width;
                }
                var columnWidth = width / dataGridView.ColumnCount;
                for (var i = 0; i < dataGridView.ColumnCount; i++)
                {
                    dataGridView.Columns[i].Width = i == dataGridView.ColumnCount - 1?
                                                    columnWidth + (width - (columnWidth * dataGridView.ColumnCount)) :
                                                    columnWidth;
                }
            }
            finally
            {
                dataGridView.ResumeLayout();
            }
        }

        private void grouperEventHub_CustomPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(SystemColors.ActiveBorder, 1),
                                    cboEventHub.Location.X - 1,
                                    cboEventHub.Location.Y - 1,
                                    cboEventHub.Size.Width + 1,
                                    cboEventHub.Size.Height + 1);
        }
        #endregion
    }
}
