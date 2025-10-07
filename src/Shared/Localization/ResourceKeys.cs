namespace ZPL2PDF.Shared.Localization
{
    /// <summary>
    /// Contains all resource keys for localization
    /// </summary>
    public static class ResourceKeys
    {
        // Application info
        public const string APPLICATION_NAME = "APPLICATION_NAME";
        public const string APPLICATION_DESCRIPTION = "APPLICATION_DESCRIPTION";
        
        // Help and usage
        public const string HELP_TITLE = "HELP_TITLE";
        public const string HELP_DEFAULT_BEHAVIOR = "HELP_DEFAULT_BEHAVIOR";
        public const string HELP_DEFAULT_USAGE = "HELP_DEFAULT_USAGE";
        public const string HELP_DEFAULT_DESCRIPTION = "HELP_DEFAULT_DESCRIPTION";
        public const string HELP_CONVERSION_MODE = "HELP_CONVERSION_MODE";
        public const string HELP_CONVERSION_USAGE = "HELP_CONVERSION_USAGE";
        public const string HELP_DAEMON_MODE = "HELP_DAEMON_MODE";
        public const string HELP_DAEMON_USAGE = "HELP_DAEMON_USAGE";
        
        // Daemon messages
        public const string DAEMON_STARTED_SUCCESS = "DAEMON_STARTED_SUCCESS";
        public const string DAEMON_STOPPED_SUCCESS = "DAEMON_STOPPED_SUCCESS";
        public const string DAEMON_ALREADY_RUNNING = "DAEMON_ALREADY_RUNNING";
        public const string DAEMON_NOT_RUNNING = "DAEMON_NOT_RUNNING";
        public const string DAEMON_STATUS_RUNNING = "DAEMON_STATUS_RUNNING";
        public const string STARTING_DAEMON = "STARTING_DAEMON";
        public const string MONITORING_FOLDER = "MONITORING_FOLDER";
        public const string PRESS_CTRL_C_TO_STOP = "PRESS_CTRL_C_TO_STOP";
        
        // File processing
        public const string FILE_PROCESSED_SUCCESS = "FILE_PROCESSED_SUCCESS";
        public const string FILE_PROCESSING_ERROR = "FILE_PROCESSING_ERROR";
        public const string CONVERSION_SUCCESS = "CONVERSION_SUCCESS";
        public const string CONVERSION_ERROR = "CONVERSION_ERROR";
        public const string FILE_NOT_FOUND = "FILE_NOT_FOUND";
        public const string FOLDER_CREATED = "FOLDER_CREATED";
        
        // Information messages
        public const string DIMENSIONS_INFO = "DIMENSIONS_INFO";
        public const string PRINT_DENSITY_INFO = "PRINT_DENSITY_INFO";
        
        // Error messages
        public const string INVALID_ARGUMENTS = "INVALID_ARGUMENTS";
        
        // Help detailed messages
        public const string HELP_PARAMETERS = "HELP_PARAMETERS";
        public const string HELP_COMMANDS = "HELP_COMMANDS";
        public const string HELP_DAEMON_OPTIONS = "HELP_DAEMON_OPTIONS";
        public const string HELP_SHOW_HELP_MESSAGE = "HELP_SHOW_HELP_MESSAGE";
        public const string HELP_DAEMON_DIRECT_OPTIONS = "HELP_DAEMON_DIRECT_OPTIONS";
        
        // Help parameters
        public const string HELP_PARAM_INPUT_FILE = "HELP_PARAM_INPUT_FILE";
        public const string HELP_PARAM_ZPL_CONTENT = "HELP_PARAM_ZPL_CONTENT";
        public const string HELP_PARAM_OUTPUT_FOLDER = "HELP_PARAM_OUTPUT_FOLDER";
        public const string HELP_PARAM_OUTPUT_NAME = "HELP_PARAM_OUTPUT_NAME";
        public const string HELP_PARAM_WIDTH = "HELP_PARAM_WIDTH";
        public const string HELP_PARAM_HEIGHT = "HELP_PARAM_HEIGHT";
        public const string HELP_PARAM_DENSITY = "HELP_PARAM_DENSITY";
        public const string HELP_PARAM_UNIT = "HELP_PARAM_UNIT";
        
        // Help commands
        public const string HELP_CMD_START = "HELP_CMD_START";
        public const string HELP_CMD_STOP = "HELP_CMD_STOP";
        public const string HELP_CMD_STATUS = "HELP_CMD_STATUS";
        public const string HELP_CMD_RUN = "HELP_CMD_RUN";
        
        // Help daemon options
        public const string HELP_OPT_LISTEN_FOLDER = "HELP_OPT_LISTEN_FOLDER";
        public const string HELP_OPT_WIDTH_FIXED = "HELP_OPT_WIDTH_FIXED";
        public const string HELP_OPT_HEIGHT_FIXED = "HELP_OPT_HEIGHT_FIXED";
        public const string HELP_OPT_UNIT_DAEMON = "HELP_OPT_UNIT_DAEMON";
        public const string HELP_OPT_DENSITY_DAEMON = "HELP_OPT_DENSITY_DAEMON";
        public const string HELP_LANGUAGE_PARAM = "HELP_LANGUAGE_PARAM";
        
        // Additional daemon messages
        public const string CHECKING_DAEMON_STATUS = "CHECKING_DAEMON_STATUS";
        public const string STOPPING_DAEMON = "STOPPING_DAEMON";
        public const string DAEMON_STARTED_BUT_FAILED_PID = "DAEMON_STARTED_BUT_FAILED_PID";
        public const string FAILED_TO_START_DAEMON = "FAILED_TO_START_DAEMON";
        public const string ERROR_STOPPING_DAEMON = "ERROR_STOPPING_DAEMON";
        public const string INVALID_PID_IN_FILE = "INVALID_PID_IN_FILE";
        
        // Processing messages
        public const string NO_IMAGES_GENERATED = "NO_IMAGES_GENERATED";
        public const string STARTING_QUEUE_PROCESSING = "STARTING_QUEUE_PROCESSING";
        public const string QUEUE_PROCESSING_STOPPED = "QUEUE_PROCESSING_STOPPED";
        public const string MONITORING_STOPPED = "MONITORING_STOPPED";
        public const string PROCESSING_FILES_LIST_CLEARED = "PROCESSING_FILES_LIST_CLEARED";
        
        // Configuration messages
        public const string MONITORING_FOLDER_NOT_CONFIGURED = "MONITORING_FOLDER_NOT_CONFIGURED";
        public const string LABEL_WIDTH_MUST_BE_GREATER_THAN_ZERO = "LABEL_WIDTH_MUST_BE_GREATER_THAN_ZERO";
        public const string LABEL_HEIGHT_MUST_BE_GREATER_THAN_ZERO = "LABEL_HEIGHT_MUST_BE_GREATER_THAN_ZERO";
        public const string DPI_MUST_BE_GREATER_THAN_ZERO = "DPI_MUST_BE_GREATER_THAN_ZERO";
        public const string INVALID_UNIT_CONFIG = "INVALID_UNIT_CONFIG";
        public const string INVALID_LOG_LEVEL_CONFIG = "INVALID_LOG_LEVEL_CONFIG";
        public const string CURRENT_ZPL2PDF_CONFIGURATION = "CURRENT_ZPL2PDF_CONFIGURATION";
        public const string MONITORING_FOLDER_CONFIG = "MONITORING_FOLDER_CONFIG";
        
        // Language configuration commands
        public const string HELP_SET_LANGUAGE = "HELP_SET_LANGUAGE";
        public const string HELP_RESET_LANGUAGE = "HELP_RESET_LANGUAGE";
        public const string HELP_SHOW_LANGUAGE = "HELP_SHOW_LANGUAGE";
        public const string INVALID_LANGUAGE_CODE = "INVALID_LANGUAGE_CODE";
        public const string SUPPORTED_LANGUAGES_LIST = "SUPPORTED_LANGUAGES_LIST";
        public const string LANGUAGE_SET_SUCCESS = "LANGUAGE_SET_SUCCESS";
        public const string LANGUAGE_RESET_SUCCESS = "LANGUAGE_RESET_SUCCESS";
        public const string RESTART_REQUIRED = "RESTART_REQUIRED";
        public const string ERROR_SETTING_LANGUAGE = "ERROR_SETTING_LANGUAGE";
        public const string ERROR_RESETTING_LANGUAGE = "ERROR_RESETTING_LANGUAGE";
        public const string CURRENT_LANGUAGE_CONFIG = "CURRENT_LANGUAGE_CONFIG";
        public const string ENV_VAR_LANGUAGE = "ENV_VAR_LANGUAGE";
        public const string ENV_VAR_NOT_SET = "ENV_VAR_NOT_SET";
        public const string CURRENT_LANGUAGE = "CURRENT_LANGUAGE";
        public const string SYSTEM_LANGUAGE = "SYSTEM_LANGUAGE";
        public const string LANGUAGE_PRIORITY_ORDER = "LANGUAGE_PRIORITY_ORDER";
    }
}
