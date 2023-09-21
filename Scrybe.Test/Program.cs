using Scrybe;

var d = DateTime.Now;
string loggingLevel = "I";
var program = new Scrybe<DateTime>();
bool exit = false;
int methodValue = LogMethod();
program.LogVariableValue(nameof(methodValue), methodValue);
Console.WriteLine("Begin logging");
program.LogTimeElapsed(d);
while(!exit)
{
    string? msg = Console.ReadLine();
    SetLoggingLevel(msg);
    Log(msg);
    exit = string.Equals(msg, "exit", StringComparison.InvariantCultureIgnoreCase);
}


int LogMethod()
{
    program.LogMethodStart();
    return program.LogMethodEnd(56);
}


void SetLoggingLevel(string? i)
{
    switch(i)
    {
        case "I":
        case "D":
        case "E":
        case "F":
        case "T":
        case "X":
        case "W":
        case "V":
            Console.WriteLine($"Logging level set to {i}");
            loggingLevel = i;
            break;
    }
}


void Log(object? msg)
{
    switch(loggingLevel)
    {
        case "I":
            program.LogInfo(msg);
            break;

        case "D":
            program.LogDebug(msg);
            break;

        case "E":
            program.LogError(msg);
            break;

        case "F":
            program.LogFatal(msg);
            break;

        case "T":
            program.LogTrace(msg);
            break;

        case "W":
            program.LogWarning(msg);
            break;

        case "V":
            program.LogVerbose(msg);
            break;

        case "X":
            program.LogCustom(msg, int.MaxValue);
            break;

    }
}