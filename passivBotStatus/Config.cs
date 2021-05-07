namespace passivBotCheck {

    public class Config {

        public string BatchPath { get; set; }
        public string LocalSharedFolder { get; set; }
        public string ExternalSharedFolder { get; set; }
        public bool Backup { get; set; }
        public int Timeout { get; set; } // Seconds
        public int CheckFrequency { get; set; } // Seconds - Should be lower than timeout
        public int UpdateFrequency { get; set; } // Seconds - Should be lower than timeout

    }
}
