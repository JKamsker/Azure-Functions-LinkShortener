using System;
using System.Collections.Generic;
using System.Text;

namespace LinkShortener.Models.Settings
{
    public class DefaultAdminSettings
    {
        public string AdminKey { get; set; }

        public DefaultAdminSettings(string adminKey)
        {
            AdminKey = adminKey;
        }

        public DefaultAdminSettings()
        {
            
        }
    }
}