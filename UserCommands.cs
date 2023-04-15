//namespace UserCommands;

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
    public int number;
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
        this.cmdType = command.uncknown;
        text = text.Trim().ToLower();
        string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
        {
            this.cmdType = command.uncknown;
            return command.uncknown;
        }
        string cmd = parts[0];

        if (cmd == "help" || cmd == "пом") return ParseSimple(text, command.help);
        else if (cmd == "reset" || cmd == "сброс") return ParseSimple(text, command.reset);
        else if (cmd == "excl" || cmd == "искл") return ParseWithCharArray(text, command.exclude);
        else if (cmd == "req" || cmd == "надо") return ParseWithCharArray(text, command.require);
        else if (cmd == "pos" || cmd == "поз") return ParseToPosition(text, command.position);
        else if (cmd == "show" || cmd == "показ") return ParseSimple(text, command.show);
        else if (cmd == "rand" || cmd == "случ") return ParseWithNumber(text, command.random);
        else if (cmd == "qu" || cmd == "вых") return ParseSimple(text, command.quit);
        //Console.WriteLine(input);

        return command.uncknown;
    }


    /// для комманды сортировки по позиции символа
    private command ParseToPosition(string text, command cmdType)
    {
        // разделяем текст на части по пробелам
        string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // количество частей должно быть 3 или 4
        if (parts.Length < 3 || parts.Length > 4) return command.uncknown;
        // если 4, то дожен быть параметр "не"
        if (parts.Length == 4 && (parts[2] != "не" && parts[2] != "no")) return command.uncknown;

        //if (parts[0] != "pos" && parts[0] != "поз") return command.uncknown;

        // вторая часть это символ, поэтому длинна её должна быть 1 символ
        if (parts[1].Length != 1) return command.uncknown;
        // и этот символ должне быть буквой русского алфавита
        char letter = parts[1][0];
        if (letter < 'а' || letter > 'я') return command.uncknown;

        // последей частью должна быть позиция символла, поэтому это однин символ и он - число от 1 до 5
        if (parts[parts.Length - 1].Length > 1) return command.uncknown;
        char numChar = parts[parts.Length - 1][0];
        if (numChar < '1' || numChar > '5') return command.uncknown;

        int index = numChar - '0';
        if (parts.Length == 3) this.position = index;
        else this.position = 0 - index;

        this.letter = letter;

        this.cmdType = cmdType;
        return cmdType;
    }


    /// для простых команд без аргументов
    private command ParseSimple(string text, command cmdType)
    {
        string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 1) return command.uncknown;

        this.cmdType = cmdType;
        return cmdType;
    }


    /// для комманд со списком символов в качестве аргумента (искл, надо)
    private command ParseWithCharArray(string text, command cmdType)
    {

        // разделяем текст на части по пробелам
        string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        // если частей всего 1, то аргументов нет
        if (parts.Length == 1) return command.uncknown;

        string arguments = string.Empty;
        for (int i = 1; i < parts.Length; i++) arguments += parts[i];      // собираем аргументы обратно в строку
        char[] symbols = arguments.ToCharArray();                          // и разбираем на символы

        // все символы должны быть буквать русского алфавита
        foreach (char item in symbols) if (item < 'а' || item > 'я') return command.uncknown;

        this.letters = symbols;     // присваиваем результат в поле letters
        this.cmdType = cmdType;
        return cmdType;     // возвращаем тип комманады в знак положительного завершения проверки
    }

    /// для команд с целочистленным аргументом 
    private command ParseWithNumber(string text, command cmdType)
    {
        // разделяем текст на части по пробелам
        string[] parts = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        // должно быть всего 2 части: команда и число
        // для "случ" может быть исключение - нет аргументов, значит число = 1
        if (cmdType == command.random && parts.Length == 1)
        {
            this.number = 1;
            return cmdType;
        }

        if (parts.Length != 2) return command.uncknown;

        // вторая часть (аргумент) должен быть числом (положительным)
        foreach (char item in parts[1].ToCharArray()) if (item < '0' || item > '9') return command.uncknown;

        this.number = int.Parse(parts[1]);
        this.cmdType = cmdType;
        return cmdType;
    }
}
