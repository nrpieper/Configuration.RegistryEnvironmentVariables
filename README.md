# Configuration.RegistryEnvironmentVariables
Add method: AddRegistryEnviromentVariables allowing Builder.Configuration to add environment variables to AppSettings without having to recycle the parent process. Useful for ASP.Net Core applications, so IIS doesn't have to be restarted when an environment variable value is added or changed.
