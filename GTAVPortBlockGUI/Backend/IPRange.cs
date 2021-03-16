using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Windows;
using System.Text.RegularExpressions;

namespace PortBlock.IPRange
{
    class IPRange
    {
        //Ip address lenght function
        public static string RangeIps(SortedDictionary<string, bool> blockList)
        {
            List<uint> byteIpList = new List<uint>(); //Parsed IP to Byte
            List<string> rangedblockList = new List<string>(); //Parsed IP Range
            rangedblockList.Add("0.0.0.0");
            
            SortedDictionary<uint, bool> rangedByteList = new SortedDictionary<uint, bool>(); //Byte IP Range
            if (blockList.Any())
            {
                foreach (KeyValuePair<string, bool> ip in blockList)
                {
                    var address = IPAddress.Parse(ip.Key);
                    byte[] bytes = address.GetAddressBytes();
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    //MessageBox.Show(BitConverter.ToUInt32(bytes, 0).ToString()); ;
                    byteIpList.Add(BitConverter.ToUInt32(bytes, 0));
                }
                byteIpList.Sort();
                foreach (uint byteAddress in byteIpList)
                {
                    rangedByteList.Add(byteAddress - 1, true);
                    rangedByteList.Add(byteAddress + 1, true);
                    //MessageBox.Show((byteAddress - 1).ToString());

                }
                byteIpList.Clear();
                foreach (KeyValuePair<uint, bool> byteAddress in rangedByteList)
                {

                    byte[] bytes = BitConverter.GetBytes(byteAddress.Key);
                    if (BitConverter.IsLittleEndian)
                    {
                        Array.Reverse(bytes);
                    }
                    //MessageBox.Show(new IPAddress(bytes).ToString());
                    rangedblockList.Add(new IPAddress(bytes).ToString());
                }
                rangedblockList.Add("255.255.255.255");
                string pattern = @"(.*?\-.*?)[\-]";
                string replacement = @"$+,";
                string input = string.Join("-", rangedblockList.ToArray());
                return (Regex.Replace(input, pattern, replacement));
            }
            else
            {
                return null;
            }
        }
    }
}
