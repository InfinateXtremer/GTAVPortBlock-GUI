using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NetFwTypeLib;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.Win32;
using System.Net;
using PortBlock.FireWall;
using PortBlock.IPRange;
using System.Collections.Specialized;

namespace GTAVPortBlockGUI
{
    public partial class MainWindow : Window
    {
        public SortedList<string, bool> blockList = new SortedList<string, bool>();
        

        public MainWindow()
        {
            InitializeComponent();
            CheckForGTAVLocation();
            CheckFirewall();
            GetIpListSetting();
            ipListbox.ItemsSource = blockList;
        }
        //AddIpAddress
        void AddAdressButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateIPv4(addressBox.Text))
            {

                try
                {
                    blockList.Add(addressBox.Text, true);
                    Properties.Settings.Default.blockList.Add(addressBox.Text);
                    Properties.Settings.Default.Save();
                    ipListbox.Items.Refresh();
                }
                catch (Exception f)
                {
                    MessageBox.Show(f.Message);
                    addressBox.Text = ("");
                }

            }
            else
            {
                MessageBox.Show("This is not a valid ip address");
                addressBox.Text = ("");
            }
        }
        //REMOVE IP BUTTON FUNCTIONS
        private void RemoveIP_Click(object sender, RoutedEventArgs e)
        {
            if (ipListbox.SelectedItems.Count >= 1)
            {
                foreach (KeyValuePair<string, bool> ip in ipListbox.SelectedItems)
                {
                    blockList.Remove(ip.Key);
                    Properties.Settings.Default.blockList.Remove(ip.Key);
                }
                Properties.Settings.Default.Save();
                ipListbox.Items.Refresh();
            }
            else
            {
                MessageBox.Show("Select the IPs first");
            }
        }
        void GetIpListSetting()
        {
            if(Properties.Settings.Default.blockList != null) // try getting blocklist string collection
            { 
                foreach (string ip in Properties.Settings.Default.blockList)
                {
                    blockList.Add(ip, true);
                }
            }
            else //init string collection if is not vallid
            {
                StringCollection sc = new StringCollection();
                Properties.Settings.Default.blockList = sc;
                Properties.Settings.Default.Save();
            }

        }
        private void ruleToggle_Click(object sender, RoutedEventArgs e)
        {   
            string firewallRange = IPRange.RangeIps(blockList);
            //MessageBox.Show(firewallRange);
            Firewall.CreateFirewallRule(firewallRange);
        }
        void CheckFirewall()
        {
            if (Firewall.CheckFirewallEnabled())
            {
                firewallLabel.Content = "Firewall Enabled";
                firewallLabel.Foreground = Brushes.Green;
            }
            else
            {
                firewallLabel.Content = "Firewall Not Enabled";
                firewallLabel.Foreground = Brushes.Red;
            }
        }

        private void firewallToggle_Click(object sender, RoutedEventArgs e)
        {
            string strCmdText;

            if (Firewall.CheckFirewallEnabled())
            {
                strCmdText = "/C NetSh Advfirewall set allprofiles state off";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                CheckFirewall();
            }
            else
            {
                strCmdText = "/C NetSh Advfirewall set allprofiles state on";
                System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                CheckFirewall();
            }
        }

        //IP ADDRESS BOX FUNCTIONS
        public bool ValidateIPv4(string ipString)
        {
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }
            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }
            byte tempForParsing;
            return splitValues.All(r => byte.TryParse(r, out tempForParsing));
        }

        Regex regex = new Regex("[^0-9.]+");
        private void addressBox_GotFocus(object sender, RoutedEventArgs e)
        {
            
            if (addressBox.Text.Length > 1 || !regex.IsMatch(addressBox.ToString())) { addressBox.Text = ""; }
        }
        private void addressBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (addressBox.Text.Length < 1 ) { addressBox.Text = "IP Address"; }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }

        //Install Location Related Functions
        void CheckForGTAVLocation()
        {
            if (!File.Exists(Properties.Settings.Default.GTAVEXE) && Properties.Settings.Default.GTAVEXE.Contains("GTA5.EXE"));
            { 
                RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\WOW6432Node\\Rockstar Games\\GTAV");
                if (registryKey == null)
                {
                    MessageBox.Show("GTA V Registry not found, You need to manually find GTA V");
                    OpenGTAVFileBrowser();
                }
                else
                {
                    Object o = registryKey.GetValue("InstallFolderSteam");
                    if (o == null)
                    {
                        MessageBox.Show("GTA V Registry Value not found, You need to manually find GTA V");
                        OpenGTAVFileBrowser();
                    }
                    else
                    {
                        int index = o.ToString().LastIndexOf("GTAV"); //remove GTAV from registry key
                        if (File.Exists(o.ToString().Substring(0, index) + "GTA5.exe")) //combine registry with exe name
                        {
                            Properties.Settings.Default.GTAVEXE = (o.ToString().Substring(0, index) + "GTA5.exe");
                            //MessageBox.Show(GTAVEXE.ToString());
                            gtavInstallLocation.Text = Properties.Settings.Default.GTAVEXE;
                            Properties.Settings.Default.Save();
                        }
                        else
                        {
                            MessageBox.Show("Cannot find GTA5.exe");
                            OpenGTAVFileBrowser();
                        }

                    }
                }
            }
        }

        void OpenGTAVFileBrowser()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "GTA5.EXE |GTA5.exe|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                Properties.Settings.Default.GTAVEXE = openFileDialog.FileName;
                gtavInstallLocation.Text = Properties.Settings.Default.GTAVEXE;
                Properties.Settings.Default.Save();
            }
        }
        private void installLocation_Click(object sender, RoutedEventArgs e)
        {
            OpenGTAVFileBrowser();
        }
        private void locationsearch_click(object sender, RoutedEventArgs e)
        {
            CheckForGTAVLocation();
        }

        private void ruleDisable_Click(object sender, RoutedEventArgs e)
        {
            Firewall.RemoveRules();
        }
    }
}
