namespace Tests;
using UserCommands;

public static class Test
{
    public static void RunTest()
    {
        UserCommand uc = new UserCommand();

        string userText = Console.ReadLine()!;
        UserCommand.command cmd = uc.GetCommand(userText);
        while(cmd != UserCommand.command.quit)
        {
            switch (cmd)
            {
                case UserCommand.command.help:
                case UserCommand.command.reset:
                case UserCommand.command.show:
                case UserCommand.command.quit:
                case UserCommand.command.uncknown:
                    Console.WriteLine(cmd);
                break;
                case UserCommand.command.exclude:
                case UserCommand.command.require:
                    Console.WriteLine(cmd + " " + new string(uc.letters));
                break;
                case UserCommand.command.random:
                    Console.WriteLine(cmd + " " + uc.number);
                break;
                case UserCommand.command.position:
                    if (uc.position > 0) Console.WriteLine(cmd + " " + uc.letter + " " + uc.position);
                    else Console.WriteLine(cmd + " " + uc.letter + " не " + (0 - uc.position));
                break;

            }
            userText = Console.ReadLine()!;
            cmd = uc.GetCommand(userText);
        }

    }

}