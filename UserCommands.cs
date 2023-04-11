namespace UserCommands;

public class UserCommand
{
    public enum command
    {
        help,
        reset,
        exclude,
        require,
        position,
        show,
        random,
        quit,
        uncknown
    }

    public command cmdType;
    public char[] letters;
    public char letter;
    public int position;
    public bool avail;

    public UserCommand()
    {
        cmdType = command.uncknown;
        letters = new char[0];
        letter = ' ';
        position = 0;
        avail = true;
    }

    public command GetCommand(string text)
    {
        command result = command.uncknown;

        if (text == "help" || text == "пом") result = command.help;
        else if (text == "reset" || text == "сброс") result =  command.reset;
        else if (text.IndexOf("excl") == 0 || text.IndexOf("искл") == 0) result =  command.exclude;
        else if (text.IndexOf("req") == 0 || text.IndexOf("надо") == 0) result =  command.require;
        else if (text.IndexOf("pos") == 0 || text.IndexOf("поз") == 0) result = command.position;
        else if (text == "show" || text == "показ") result =  command.show;
        else if (text == "rand" || text == "случ") result =  command.random;
        else if (text == "qu" || text == "вых") result =  command.quit;
        //Console.WriteLine(input);
        
        // разбираем комманду на аргументы
        return Parse(text, result);
    }

    private command Parse(string text, command cmdType)
    {   
        command result = cmdType;
        switch (cmdType)
        {
            case command.help:
            case command.reset:
            case command.show:
            case command.random:
            case command.quit:
                this.cmdType = cmdType;
                return cmdType;
            //break;
        }

        // разбираем command.exclude
        if(cmdType == command.exclude)
        {


        }



        this.cmdType = cmdType;
        return result;
    }

    private char[] GetCharParams(string input)               /// !!!!!!!!!!!!!!!!!!!!!
    {
        // 1. обрезаем комманду, оставляем только символы
        int del = 4;
        if (input.IndexOf("req") == 0) del = 3;
        //else if (input.IndexOf("req") == 0)    del=3;
        input = (input.Remove(0, del)).Trim();

        // 2. создаем из остатка массив символов
        char[] chars = input.ToCharArray();

        // 3. из этого массива оставляем только "наши" буквы
        char[] result = new char[0];
        foreach (char ch in chars)
        {
            if (ch >= 'а' && ch <= 'я')
            {
                Array.Resize(ref result, result.Length + 1);
                result[result.Length - 1] = ch;
            }
        }

        return result;
    }

}
