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
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Forms;
using Microsoft.AzureCat.Samples.PayloadEntities;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#endregion

namespace Microsoft.AzureCat.Samples.DeviceSimulator
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
        private const string LogFileNameFormat = "DeviceSimulatorLog-{0}.txt";
        private const string EventHubUrl = "https://{0}.servicebus.windows.net/{1}/publishers/{2}";

        //***************************
        // Constants
        //***************************
        private const string SaveAsTitle = "Save Log As";
        private const string SaveAsExtension = "txt";
        private const string SaveAsFilter = "Text Documents (*.txt)|*.txt";
        private const string Start = "Start";
        private const string Stop = "Stop";
        private const string SenderSharedAccessKey = "SenderSharedAccessKey";
        private const string DeviceId = "id";
        private const string DeviceName = "name";
        private const string DeviceStatus = "status";
        private const string Value = "value";
        private const string Timestamp = "timestamp";

        //***************************
        // Configuration Parameters
        //***************************
        private const string UrlParameter = "url";
        private const string NamespaceParameter = "namespace";
        private const string KeyNameParameter = "keyName";
        private const string KeyValueParameter = "keyValue";
        private const string EventHubParameter = "eventHub";
        private const string DeviceCountParameter = "deviceCount";
        private const string EventIntervalParameter = "eventInterval";
        private const string MinValueParameter = "minValue";
        private const string MaxValueParameter = "maxValue";
        private const string MinOffsetParameter = "minOffset";
        private const string MaxOffsetParameter = "maxOffset";
        private const string SpikePercentageParameter = "spikePercentage";
        private const string ApiVersion = "&api-version=2014-05";

        //***************************
        // Configuration Parameters
        //***************************
        private const string DefaultEventHubName = "DeviceDemoInputHub";
        private const string DefaultStatus = "Ok";
        private const int DefaultDeviceNumber = 10;
        private const int DefaultMinValue = 20;
        private const int DefaultMaxValue = 50;
        private const int DefaultMinOffset = 20;
        private const int DefaultMaxOffset = 50;
        private const int DefaultSpikePercentage = 10;
        private const int DefaultEventIntervalInMilliseconds = 100;


        //***************************
        // Messages
        //***************************
        private const string UrlCannotBeNull = "The device management service URL cannot be null.";
        private const string NamespaceCannotBeNull = "The Service Bus namespace cannot be null.";
        private const string EventHubNameCannotBeNull = "The Event Hub name cannot be null.";
        private const string KeyNameCannotBeNull = "The Key name cannot be null.";
        private const string KeyValueCannotBeNull = "The Key value cannot be null.";
        private const string EventHubCreatedOrRetrieved = "Event Hub [{0}] successfully retrieved.";
        private const string MessagingFactoryCreated = "Device[{0,3:000}]. MessagingFactory created.";
        private const string SasToken = "Device[{0,3:000}]. SAS Token created.";
        private const string EventHubClientCreated = "Device[{0,3:000}]. EventHubClient created: Path=[{1}].";
        private const string HttpClientCreated = "Device[{0,3:000}]. HttpClient created: BaseAddress=[{1}].";
        private const string SendFailed = "Device[{0,3:000}]. Message send failed: [{1}]";
        private const string EventHubDoesNotExists = "The Event Hub [{0}] does not exist.";
        private const string InitializingDevices = "Initializing devices...";
        private const string DevicesInitialized = "Devices initialized.";
        #endregion

        #region Private Fields
        private CancellationTokenSource cancellationTokenSource;
        private readonly Random random = new Random((int)DateTime.Now.Ticks);
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

        #region Public Methods

        public void ConfigureComponent()
        {
            txtNamespace.AutoSize = false;
            txtNamespace.Size = new Size(txtNamespace.Size.Width, 24);
            txtKeyName.AutoSize = false;
            txtKeyName.Size = new Size(txtKeyName.Size.Width, 24);
            txtKeyValue.AutoSize = false;
            txtKeyValue.Size = new Size(txtKeyValue.Size.Width, 24);
            cboEventHub.AutoSize = false;
            cboEventHub.Size = new Size(cboEventHub.Size.Width, 24);
            txtDeviceCount.AutoSize = false;
            txtDeviceCount.Size = new Size(txtDeviceCount.Size.Width, 24);
            txtEventIntervalInMilliseconds.AutoSize = false;
            txtEventIntervalInMilliseconds.Size = new Size(txtEventIntervalInMilliseconds.Size.Width, 24);
            txtMinValue.AutoSize = false;
            txtMinValue.Size = new Size(txtMinValue.Size.Width, 24);
            txtMaxValue.AutoSize = false;
            txtMaxValue.Size = new Size(txtMaxValue.Size.Width, 24);
            txtMinOffset.AutoSize = false;
            txtMinOffset.Size = new Size(txtMinOffset.Size.Width, 24);
            txtMaxOffset.AutoSize = false;
            txtMaxOffset.Size = new Size(txtMinOffset.Size.Width, 24);
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
                var urlValue = ConfigurationManager.AppSettings[UrlParameter] ?? DefaultEventHubName;
                var urls = urlValue.Split(',', ';');
                foreach (var url in urls)
                {
                    cboDeviceManagementServiceUrl.Items.Add(url);
                }
                cboDeviceManagementServiceUrl.SelectedIndex = 0;
                txtNamespace.Text = ConfigurationManager.AppSettings[NamespaceParameter];
                txtKeyName.Text = ConfigurationManager.AppSettings[KeyNameParameter];
                txtKeyValue.Text = ConfigurationManager.AppSettings[KeyValueParameter];
                var eventHubValue = ConfigurationManager.AppSettings[EventHubParameter] ?? DefaultEventHubName;
                var eventHubs = eventHubValue.Split(',', ';');
                foreach (var eventHub in eventHubs)
                {
                    cboEventHub.Items.Add(eventHub);
                }
                cboEventHub.SelectedIndex = 0;
                int value;
                var setting = ConfigurationManager.AppSettings[DeviceCountParameter];
                txtDeviceCount.Text = int.TryParse(setting, out value) ? 
                                       value.ToString(CultureInfo.InvariantCulture) : 
                                       DefaultDeviceNumber.ToString(CultureInfo.InvariantCulture);
                setting = ConfigurationManager.AppSettings[EventIntervalParameter];
                txtEventIntervalInMilliseconds.Text = int.TryParse(setting, out value) ?
                                       value.ToString(CultureInfo.InvariantCulture) :
                                       DefaultEventIntervalInMilliseconds.ToString(CultureInfo.InvariantCulture);
                setting = ConfigurationManager.AppSettings[MinValueParameter];
                txtMinValue.Text = int.TryParse(setting, out value) ?
                                       value.ToString(CultureInfo.InvariantCulture) :
                                       DefaultMinValue.ToString(CultureInfo.InvariantCulture);
                setting = ConfigurationManager.AppSettings[MaxValueParameter];
                txtMaxValue.Text = int.TryParse(setting, out value) ?
                                       value.ToString(CultureInfo.InvariantCulture) :
                                       DefaultMaxValue.ToString(CultureInfo.InvariantCulture);
                setting = ConfigurationManager.AppSettings[MinOffsetParameter];
                txtMinOffset.Text = int.TryParse(setting, out value) ?
                                       value.ToString(CultureInfo.InvariantCulture) :
                                       DefaultMinOffset.ToString(CultureInfo.InvariantCulture);
                setting = ConfigurationManager.AppSettings[MaxOffsetParameter];
                txtMaxOffset.Text = int.TryParse(setting, out value) ?
                                       value.ToString(CultureInfo.InvariantCulture) :
                                       DefaultMaxOffset.ToString(CultureInfo.InvariantCulture);
                setting = ConfigurationManager.AppSettings[SpikePercentageParameter];
                trackbarSpikePercentage.Value = int.TryParse(setting, out value)
                                                ? value
                                                : DefaultSpikePercentage;
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
            var width = (mainHeaderPanel.Size.Width - 80)/2;
            var halfWidth = (width - 16)/2;
            var panelWidth = mainHeaderPanel.Size.Width - 32;
            var panelHeight = (mainHeaderPanel.Size.Height - 192)/2;

            txtNamespace.Size = new Size(width, txtNamespace.Size.Height);
            txtKeyName.Size = new Size(width, txtKeyName.Size.Height);
            txtKeyValue.Size = new Size(width, txtKeyValue.Size.Height);
            cboEventHub.Size = new Size(width, cboEventHub.Size.Height);
            txtMinOffset.Size = new Size(halfWidth, txtMinOffset.Size.Height);
            txtMaxOffset.Size = new Size(halfWidth, txtMaxOffset.Size.Height);
            txtDeviceCount.Size = new Size(halfWidth, txtDeviceCount.Size.Height);
            txtEventIntervalInMilliseconds.Size = new Size(halfWidth, txtEventIntervalInMilliseconds.Size.Height);
            txtMinValue.Size = new Size(halfWidth, txtMinValue.Size.Height);
            txtMaxValue.Size = new Size(halfWidth, txtMaxValue.Size.Height);
            trackbarSpikePercentage.Size = new Size(width, trackbarSpikePercentage.Size.Height);

            cboEventHub.Location = new Point(32 + width, cboEventHub.Location.Y);
            txtKeyValue.Location = new Point(32 + width, txtKeyValue.Location.Y);
            txtMaxValue.Location = new Point(32 + halfWidth, txtMaxValue.Location.Y);
            txtMinOffset.Location = new Point(32 + width, txtMaxOffset.Location.Y);
            txtMaxOffset.Location = new Point(48 + + width + halfWidth, txtMaxOffset.Location.Y);
            txtEventIntervalInMilliseconds.Location = new Point(32 + halfWidth, txtEventIntervalInMilliseconds.Location.Y);
            trackbarSpikePercentage.Location = new Point(32 + width, trackbarSpikePercentage.Location.Y);

            lblEventHub.Location = new Point(32 + width, lblEventHub.Location.Y);
            lblKeyValue.Location = new Point(32 + width, lblKeyValue.Location.Y);
            lblMaxValue.Location = new Point(32 + halfWidth, lblMaxValue.Location.Y);
            lblMinOffset.Location = new Point(32 + width, lblMaxOffset.Location.Y);
            lblMaxOffset.Location = new Point(48 + +width + halfWidth, lblMaxOffset.Location.Y);
            lblEventIntervalInMilliseconds.Location = new Point(32 + halfWidth, lblEventIntervalInMilliseconds.Location.Y);
            lblSpikePercentage.Location = new Point(32 + width, lblSpikePercentage.Location.Y);
            radioButtonHttps.Location = new Point(32 + halfWidth, radioButtonAmqp.Location.Y);

            grouperEventHub.Size = new Size(panelWidth, panelHeight);
            grouperDevice.Size = new Size(panelWidth, panelHeight);
            grouperDevice.Location = new Point(16, 136 + panelHeight);
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            txtNamespace.SelectionLength = 0;
        }

        // ReSharper disable once FunctionComplexityOverflow
        private async void btnStart_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            try
            {
                if (string.Compare(btnStart.Text, Start, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    
                    // Validate parameters
                    if (!ValidateParameters())
                    {
                        return;
                    }

                    // Initialize Devices
                    if (chkInitializeDevices.Checked)
                    {
                        await InitializeDevicesAsync();
                        chkInitializeDevices.Checked = false;
                    }

                    // Start Devices
                    StartDevices();

                    // Change button text
                    btnStart.Text = Stop;
                }
                else
                {
                    // Stop Devices
                    StopDevices();

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
            }
        }

        private bool ValidateParameters()
        {
            if (string.IsNullOrWhiteSpace(cboDeviceManagementServiceUrl.Text))
            {
                WriteToLog(UrlCannotBeNull);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtNamespace.Text))
            {
                WriteToLog(NamespaceCannotBeNull);
                return false;
            }
            if (string.IsNullOrWhiteSpace(cboEventHub.Text))
            {
                WriteToLog(EventHubNameCannotBeNull);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtKeyName.Text))
            {
                WriteToLog(KeyNameCannotBeNull);
                return false;
            }
            if (string.IsNullOrWhiteSpace(txtKeyValue.Text))
            {
                WriteToLog(KeyValueCannotBeNull);
                return false;
            }
            return true;
        }

        public static string CreateSasTokenForAmqpSender(string senderKeyName, 
                                                         string senderKey, 
                                                         string serviceNamespace, 
                                                         string hubName, 
                                                         string publisherName, 
                                                         TimeSpan tokenTimeToLive)
        {
            // This is the format of the publisher endpoint. Each device uses a different publisher endpoint.
            // sb://<NAMESPACE>.servicebus.windows.net/<EVENT_HUB_NAME>/publishers/<PUBLISHER_NAME>. 
            var serviceUri = ServiceBusEnvironment.CreateServiceUri("sb", 
                                                                    serviceNamespace,
                $"{hubName}/publishers/{publisherName}")
                .ToString()
                .Trim('/');
            // SharedAccessSignature sr=<URL-encoded-resourceURI>&sig=<URL-encoded-signature-string>&se=<expiry-time-in-ISO-8061-format. >&skn=<senderKeyName>
            return SharedAccessSignatureTokenProvider.GetSharedAccessSignature(senderKeyName, senderKey, serviceUri, tokenTimeToLive);
        }

        // Create a SAS token for a specified scope. SAS tokens are described in http://msdn.microsoft.com/en-us/library/windowsazure/dn170477.aspx.
        private static string CreateSasTokenForHttpsSender(string senderKeyName, 
                                                           string senderKey, 
                                                           string serviceNamespace, 
                                                           string hubName, 
                                                           string publisherName, 
                                                           TimeSpan tokenTimeToLive)
        {
            // Set token lifetime. When supplying a device with a token, you might want to use a longer expiration time.
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var difference = DateTime.Now.ToUniversalTime() - origin;
            var tokenExpirationTime = Convert.ToUInt32(difference.TotalSeconds) + tokenTimeToLive.Seconds;

            // https://<NAMESPACE>.servicebus.windows.net/<EVENT_HUB_NAME>/publishers/<PUBLISHER_NAME>. 
            var uri = ServiceBusEnvironment.CreateServiceUri("https", serviceNamespace,
                $"{hubName}/publishers/{publisherName}")
                .ToString()
                .Trim('/');
            var stringToSign = HttpUtility.UrlEncode(uri) + "\n" + tokenExpirationTime;
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(senderKey));

            var signature = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(stringToSign)));

            // SharedAccessSignature sr=<URL-encoded-resourceURI>&sig=<URL-encoded-signature-string>&se=<expiry-time-in-ISO-8061-format. >&skn=<senderKeyName>
            var token = String.Format(CultureInfo.InvariantCulture, "SharedAccessSignature sr={0}&sig={1}&se={2}&skn={3}",
            HttpUtility.UrlEncode(uri), HttpUtility.UrlEncode(signature), tokenExpirationTime, senderKeyName);
            return token;
        }

        private int GetValue(int minValue,
                             int maxValue,
                             int minOffset, 
                             int maxOffset, 
                             int spikePercentage)
        {
            var value = random.Next(0, 100);
            if (value >= spikePercentage)
            {
                return random.Next(minValue, maxValue + 1);
            }
            var sign = random.Next(0, 2);
            var offset = random.Next(minOffset, maxOffset + 1);
            offset = sign == 0 ? -offset : offset;
            return random.Next(minValue, maxValue + 1) + offset;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstLog.Items.Clear();
        }

        private async void StartDevices()
        {
            // Create namespace manager
            var namespaceUri = ServiceBusEnvironment.CreateServiceUri("sb", txtNamespace.Text, string.Empty);
            var tokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(txtKeyName.Text, txtKeyValue.Text);
            var namespaceManager = new NamespaceManager(namespaceUri, tokenProvider);

            // Check if the event hub already exists, if not, create the event hub.
            if (!await namespaceManager.EventHubExistsAsync(cboEventHub.Text))
            {
                WriteToLog(string.Format(EventHubDoesNotExists, cboEventHub.Text));
                return;
            }
            var eventHubDescription = await namespaceManager.GetEventHubAsync(cboEventHub.Text);

            WriteToLog(string.Format(EventHubCreatedOrRetrieved, cboEventHub.Text));

            // Check if the SAS authorization rule used by devices to send events to the event hub already exists, if not, create the rule.
            var authorizationRule = eventHubDescription.
                                    Authorization.
                                    FirstOrDefault(r => string.Compare(r.KeyName,
                                                                        SenderSharedAccessKey,
                                                                        StringComparison.InvariantCultureIgnoreCase)
                                                                        == 0) as SharedAccessAuthorizationRule;

            if (authorizationRule == null)
            {
                authorizationRule = new SharedAccessAuthorizationRule(SenderSharedAccessKey,
                                                                         SharedAccessAuthorizationRule.GenerateRandomKey(),
                                                                         new[]
                                                                         {
                                                                                     AccessRights.Send
                                                                         });
                eventHubDescription.Authorization.Add(authorizationRule);
                await namespaceManager.UpdateEventHubAsync(eventHubDescription);
            }

            cancellationTokenSource = new CancellationTokenSource();
            var serviceBusNamespace = txtNamespace.Text;
            var eventHubName = cboEventHub.Text;
            var senderKey = authorizationRule.PrimaryKey;
            var status = DefaultStatus;
            var eventInterval = txtEventIntervalInMilliseconds.IntegerValue;
            var minValue = txtMinValue.IntegerValue;
            var maxValue = txtMaxValue.IntegerValue;
            var minOffset = txtMinOffset.IntegerValue;
            var maxOffset = txtMaxOffset.IntegerValue;
            var spikePercentage = trackbarSpikePercentage.Value;
            var cancellationToken = cancellationTokenSource.Token;

            // Create one task for each device
            for (var i = 1; i <= txtDeviceCount.IntegerValue; i++)
            {
                var deviceId = i;
#pragma warning disable 4014
#pragma warning disable 4014
                Task.Run(async () =>
#pragma warning restore 4014
                {
                    var deviceName = $"device{deviceId:000}";

                    if (radioButtonAmqp.Checked)
                    {
                        // The token has the following format: 
                        // SharedAccessSignature sr={URI}&sig={HMAC_SHA256_SIGNATURE}&se={EXPIRATION_TIME}&skn={KEY_NAME}
                        var token = CreateSasTokenForAmqpSender(SenderSharedAccessKey,
                                                                senderKey,
                                                                serviceBusNamespace,
                                                                eventHubName,
                                                                deviceName,
                                                                TimeSpan.FromDays(1));
                        WriteToLog(string.Format(SasToken, deviceId));

                        var messagingFactory = MessagingFactory.Create(ServiceBusEnvironment.CreateServiceUri("sb", serviceBusNamespace, ""), new MessagingFactorySettings
                        {
                            TokenProvider = TokenProvider.CreateSharedAccessSignatureTokenProvider(token),
                            TransportType = TransportType.Amqp
                        });
                        WriteToLog(string.Format(MessagingFactoryCreated, deviceId));

                        // Each device uses a different publisher endpoint: [EventHub]/publishers/[PublisherName]
                        var eventHubClient = messagingFactory.CreateEventHubClient($"{eventHubName}/publishers/{deviceName}");
                        WriteToLog(string.Format(EventHubClientCreated, deviceId, eventHubClient.Path));

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            // Create random value
                            var value = GetValue(minValue, maxValue, minOffset, maxOffset, spikePercentage);
                            var timestamp = DateTime.Now;

                            // Create EventData object with the payload serialized in JSON format 
                            var payload = new Payload
                            {
                                DeviceId = deviceId,
                                Name = deviceName,
                                Status = status,
                                Value = value,
                                Timestamp = timestamp
                            };
                            var json = JsonConvert.SerializeObject(payload);
                            using (var eventData = new EventData(Encoding.UTF8.GetBytes(json))
                            {
                                PartitionKey = deviceName
                            })
                            {
                                // Create custom properties
                                eventData.Properties.Add(DeviceId, deviceId);
                                eventData.Properties.Add(DeviceName, deviceName);
                                eventData.Properties.Add(DeviceStatus, status);
                                eventData.Properties.Add(Value, value);
                                eventData.Properties.Add(Timestamp, timestamp);

                                // Send the event to the event hub
                                await eventHubClient.SendAsync(eventData);
                                WriteToLog($"[Event] DeviceId=[{payload.DeviceId:000}] " +
                                           $"Value=[{payload.Value:000}] " +
                                           $"Timestamp=[{payload.Timestamp}]");
                            }

                            // Wait for the event time interval
                            Thread.Sleep(eventInterval);
                        }
                    }
                    else
                    {
                        // The token has the following format: 
                        // SharedAccessSignature sr={URI}&sig={HMAC_SHA256_SIGNATURE}&se={EXPIRATION_TIME}&skn={KEY_NAME}
                        var token = CreateSasTokenForHttpsSender(SenderSharedAccessKey,
                            senderKey,
                            serviceBusNamespace,
                            eventHubName,
                            deviceName,
                            TimeSpan.FromDays(1));
                        WriteToLog(string.Format(SasToken, deviceId));

                        // Create HttpClient object used to send events to the event hub.
                        var httpClient = new HttpClient
                        {
                            BaseAddress =
                                new Uri(string.Format(EventHubUrl,
                                    serviceBusNamespace,
                                    eventHubName,
                                    deviceName).ToLower())
                        };
                        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
                        httpClient.DefaultRequestHeaders.Add("ContentType",
                            "application/json;type=entry;charset=utf-8");
                        WriteToLog(string.Format(HttpClientCreated, deviceId, httpClient.BaseAddress));

                        while (!cancellationToken.IsCancellationRequested)
                        {
                            // Create random value
                            var value = GetValue(minValue, maxValue, minOffset, maxOffset, spikePercentage);
                            var timestamp = DateTime.Now;

                            // Create EventData object with the payload serialized in JSON format 
                            var payload = new Payload
                            {
                                DeviceId = deviceId,
                                Name = deviceName,
                                Status = status,
                                Value = value,
                                Timestamp = timestamp
                            };
                            var json = JsonConvert.SerializeObject(payload);

                            // Create HttpContent
                            var postContent = new ByteArrayContent(Encoding.UTF8.GetBytes(json));

                            // Create custom properties
                            postContent.Headers.Add(DeviceId, deviceId.ToString(CultureInfo.InvariantCulture));
                            postContent.Headers.Add(DeviceName, deviceName);
                            //postContent.Headers.Add(DeviceStatus, location);
                            postContent.Headers.Add(Value, value.ToString(CultureInfo.InvariantCulture));
                            postContent.Headers.Add(Timestamp, timestamp.ToString(CultureInfo.InvariantCulture));

                            try
                            {
                                var response =
                                    await
                                        httpClient.PostAsync(
                                            httpClient.BaseAddress + "/messages" + "?timeout=60" + ApiVersion,
                                            postContent, cancellationToken);
                                response.EnsureSuccessStatusCode();
                                WriteToLog($"[Event] DeviceId=[{payload.DeviceId:000}] " +
                                           $"Value=[{payload.Value:000}] " +
                                           $"Timestamp=[{payload.Timestamp}]");
                            }
                            catch (HttpRequestException ex)
                            {
                                WriteToLog(string.Format(SendFailed, deviceId, ex.Message));
                            }

                            // Wait for the event time interval
                            Thread.Sleep(eventInterval);
                        }
                    }
                },
                cancellationToken).ContinueWith(t =>
#pragma warning restore 4014
#pragma warning restore 4014
                        {
                    if (t.IsFaulted && t.Exception != null)
                    {
                        HandleException(t.Exception);
                    }
                }, cancellationToken);
            }
        }

        private void StopDevices()
        {
            cancellationTokenSource?.Cancel();
        }

        private async Task InitializeDevicesAsync()
        {

            var manufacturerDictionary = new Dictionary<string, List<Tuple<string, string>>>
            {
                {
                    "Contoso", new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("TS1", "Temperature Sensor"),
                        new Tuple<string, string>("TS2", "Temperature Sensor")
                    }
                }
                ,
                {
                    "Fabrikam", new List<Tuple<string, string>>
                    {
                        new Tuple<string, string>("HS1", "Humidity Sensor"),
                        new Tuple<string, string>("HS2", "Humidity Sensor")
                    }
                }
            };

            var siteDictionary = new Dictionary<string, List<string>>
            {
                {
                    "Italy",
                    new List<string> {"Milan", "Rome", "Turin"}
                },
                {
                    "Germany",
                    new List<string> {"Munich", "Berlin", "Amburg"}
                },
                {
                    "UK",
                    new List<string> {"London", "Manchester", "Liverpool"}
                },
                {
                    "France",
                    new List<string> {"Paris", "Lion", "Nice"}
                }
            };
            var deviceList = new List<Device>();
            
            // Prepare device data
            for (var i = 1; i <= txtDeviceCount.IntegerValue; i++)
            {
                var m = random.Next(0, manufacturerDictionary.Count);
                var d = random.Next(0, manufacturerDictionary.Values.ElementAt(m).Count);
                var model = manufacturerDictionary.Values.ElementAt(m)[d].Item1;
                var type = manufacturerDictionary.Values.ElementAt(m)[d].Item2;
                var s = random.Next(0, siteDictionary.Count);
                var c = random.Next(0, siteDictionary.Values.ElementAt(s).Count);

                deviceList.Add(new Device
                {
                    DeviceId = i,
                    Name = $"Device {i}",
                    MinThreshold = txtMinValue.IntegerValue,
                    MaxThreshold = txtMaxValue.IntegerValue,
                    Manufacturer = manufacturerDictionary.Keys.ElementAt(m),
                    Model = model,
                    Type = type,
                    City = siteDictionary.Values.ElementAt(s)[c],
                    Country = siteDictionary.Keys.ElementAt(s)
                });
            }

            // Create HttpClient object used to send events to the event hub.
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(cboDeviceManagementServiceUrl.Text)
            };
            httpClient.DefaultRequestHeaders.Add("ContentType", "application/json");
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var json = JsonConvert.SerializeObject(deviceList);

            // Create HttpContent
            var postContent = new StringContent(json, Encoding.UTF8, "application/json");
            WriteToLog(InitializingDevices);
            var response = await httpClient.PostAsync(Combine(httpClient.BaseAddress.AbsoluteUri, "api/devices/set"), postContent);
            response.EnsureSuccessStatusCode();
            WriteToLog(DevicesInitialized);
        }

        public static string Combine(string uri1, string uri2)
        {
            uri1 = uri1.TrimEnd('/');
            uri2 = uri2.TrimStart('/');
            return $"{uri1}/{uri2}";
        }

        private void grouperDeviceManagement_CustomPaint(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(new Pen(SystemColors.ActiveBorder, 1),
                                    cboDeviceManagementServiceUrl.Location.X - 1,
                                    cboDeviceManagementServiceUrl.Location.Y - 1,
                                    cboDeviceManagementServiceUrl.Size.Width + 1,
                                    cboDeviceManagementServiceUrl.Size.Height + 1);
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
