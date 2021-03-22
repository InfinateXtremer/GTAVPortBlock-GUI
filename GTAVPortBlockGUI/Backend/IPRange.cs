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
        public static string RangeIps(SortedList<string, bool> blockList)
        {
            List<uint> byteIpList = new List<uint>(); //Parsed IP to Byte
            List<string> rangedblockList = new List<string>(); //Parsed IP Range
            rangedblockList.Add("0.0.0.0");
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
                    byteIpList.Add(BitConverter.ToUInt32(bytes, 0) - 1); //Add ranged bytes instead of converted byte
                    byteIpList.Add(BitConverter.ToUInt32(bytes, 0) + 1); //Add ranged bytes instead of converted byte
                }
                byteIpList.Sort(); //Sort it just in case
                foreach (uint byteAddress in byteIpList)
                {
                    byte[] bytes = BitConverter.GetBytes(byteAddress);
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
