using NetFwTypeLib;
using System;
using System.Collections.Generic;


namespace PortBlock.FireWall
{
    class Firewall
    {
        static readonly string IN_RULE_NAME = "[IN-PORT-BLOCK-GTAV]";
        static readonly string OUT_RULE_NAME = "[OUT-PORT-BLOCK-GTAV]";
        static readonly string RULE_PORTS = "6672";

        //Firewall Related Functions
        public static bool CheckFirewallEnabled()
        {
            Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);
            return mgr.LocalPolicy.CurrentProfile.FirewallEnabled;
        }

        public static bool CheckRule(string rulename)
        {
        INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
        foreach (INetFwRule rule in firewallPolicy.Rules)
            if (rule.Name.IndexOf(rulename) != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
            return false;
        }

        public static void CreateFirewallRule(string IPrange)
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Remove(IN_RULE_NAME);
            firewallPolicy.Rules.Remove(OUT_RULE_NAME);
            INetFwRule2 inboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            inboundRule.Name = IN_RULE_NAME; //Name of rule
            inboundRule.Enabled = true;
            inboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK; //Block through firewall
            inboundRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            inboundRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP; // UDP
            inboundRule.RemoteAddresses = IPrange; //TODO Get ranged ip list
            inboundRule.LocalPorts = RULE_PORTS; //Gta v default session port
            inboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_IN;                       
            inboundRule.ApplicationName = GTAVPortBlockGUI.Properties.Settings.Default.GTAVEXE;
            firewallPolicy.Rules.Add(inboundRule);// Now add the rule         

            
            INetFwRule2 outboundRule = (INetFwRule2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));
            outboundRule.Name = OUT_RULE_NAME; //Name of rule
            outboundRule.Enabled = true;
            outboundRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK; //Block through firewall
            outboundRule.Profiles = (int)NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL;
            outboundRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP; // UDP
            outboundRule.RemoteAddresses = IPrange; //TODO Get ranged ip list
            outboundRule.LocalPorts = RULE_PORTS; //Gta v default session port
            outboundRule.Direction = NET_FW_RULE_DIRECTION_.NET_FW_RULE_DIR_OUT;
            outboundRule.ApplicationName = GTAVPortBlockGUI.Properties.Settings.Default.GTAVEXE;
            firewallPolicy.Rules.Add(outboundRule);// Now add the rule    
        }

        public static void RemoveRules()
        {
            INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
            firewallPolicy.Rules.Remove(IN_RULE_NAME);
            firewallPolicy.Rules.Remove(OUT_RULE_NAME);
        }

        public static void DisableFirewall()
        {

        }
    }
}
