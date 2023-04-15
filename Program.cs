using System;
using System.Linq;
using System.IO;
//using UserCommands;
using Tests;

namespace FiveLetters;

public static class Program
{
    //const string DICTONARY = "5letters.txt";
    //const string DICTONARY = "5lettersshort.txt";
    const string DICTONARY = "5letters_real.txt";
    const string HELPTEXT = "\thelp (пом) - вывести спискок комманд\n" +
                            "\treset (сброс) - сбросить все фильтры\n" +
                            "\texcl (искл) [буквы] - исключить слова с указанными буквами\n" +
                            "\treq (надо) [буквы] - оставить только слова с указанными буквами\n" +
                            "\tpos (поз) [no|не] [буква] [номер] - оставить\\исклчить слова с буквой на этой позиции\n" +
                            "\tshow (показ)- показать выбранные слова\n" +
                            "\trand (случ)- показать случайное слово\n" +
                            "\tqu (вых) - выход";
    const string ASKHUNDREDWORDS = "В выборке больше 100 слов. Показать все? ( Y | N ): ";
    static string[] allWords = new string[0];  // = ReadAllWords(allWordsFileName);

    enum command
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

    public static void Main()
    {
        // Test.RunTest();
        // return;

        //SelectFivelettersWords("букваС1.txt","5с1.txt");
        //SelectFivelettersWords("букваС2.txt","5с2.txt");
        //AllToOneFile();


        //string allWordsFileName = Directory.GetCurrentDirectory()+"/5letters.txt";
        //string allWordsFileName = Directory.GetCurrentDirectory() + "\\Letters\\5lettersshort.txt";
        //string allWordsFileName = Directory.GetCurrentDirectory() + "\\Letters\\5letters.txt";
        string allWordsFileName = Directory.GetCurrentDirectory() + "\\Letters\\" + DICTONARY;
        allWords = ReadAllWords(allWordsFileName);

        //Console.Clear();
        Console.WriteLine("Загружено {0} слов ", allWords.Length);
        Console.WriteLine("Случайное слово: {0}", GetRandonWord(allWords, 1));

        PrintHelp();
        ReadUserCommandsLoop();
        // UserCommand uc = new UserCommand();
        // string txt = Console.ReadLine()!;
        // uc.GetCommand(txt);
        // Console.WriteLine(uc.cmdType);
    }



    // запускаем цикл обработки пользовательских комманд
    private static void ReadUserCommandsLoop()
    {
        string[] filtredWords = new string[allWords.Length];
        allWords.CopyTo(filtredWords, 0);
        char[] excluded = new char[0];      // исключенные символы
        char[] required = new char[0];      // необходимые символы
        bool positionExcept = false;        // исключения по позициям символов

        UserCommand uc = new UserCommand();
        //string userText = string.Empty;
        //bool exit = false;

        Console.Write("Введите комманду: ");
        string userText = Console.ReadLine()!.Trim();
        uc.GetCommand(userText);

        //do
        while (uc.cmdType != UserCommand.command.quit)
        {
            //Console.Write("Введите комманду: ");
            //userText = Console.ReadLine()!.Trim();

            //command com = GetCommand(userText);
            //UserCommand.command com = uc.GetCommand(userText);

            //Console.WriteLine(com.ToString());
            switch (uc.cmdType)
            {
                case UserCommand.command.help:          // +++
                    // пишем хелп
                    PrintHelp();
                    break;
                case UserCommand.command.show:          // +++
                    // показываем выборку
                    if (filtredWords.Length > 100 &&    // если слов много надо уточнить
                         AckUser(ASKHUNDREDWORDS)) PrintWords(filtredWords);
                    else PrintWords(filtredWords);
                    break;
                case UserCommand.command.random:        // +++
                    // выдаём случайное слово
                    Console.WriteLine(GetRandonWord(filtredWords, uc.number));
                    break;
                case UserCommand.command.reset:         // +++
                    // сбрасываем сортировки
                    excluded = new char[0];
                    required = new char[0];
                    filtredWords = new string[allWords.Length];
                    allWords.CopyTo(filtredWords, 0);
                    positionExcept = false;
                    PrintStatus(filtredWords, excluded, required, positionExcept);
                    break;
                case UserCommand.command.quit:          // +++
                    // выходим
                    break;
                case UserCommand.command.require:       // +++
                    //char[] reqChars = GetParametres(userText);
                    char[] reqChars = uc.letters;
                    // фильтруем по необходимым буквам
                    RequiredLetters(ref filtredWords, reqChars);
                    // добавляем отфильтрованные буквы в общий список
                    AddAndSortLetters(ref required, reqChars);
                    // Выводим собщение
                    PrintStatus(filtredWords, excluded, required, positionExcept);
                    break;
                case UserCommand.command.exclude:
                    //char[] exclChars = GetParametres(userText);
                    char[] exclChars = uc.letters;
                    // фильтруем по лишним буквам
                    ExcludeLetters(ref filtredWords, exclChars);
                    // добавляем отфильтрованные буквы в общий список
                    AddAndSortLetters(ref excluded, exclChars);
                    // Выводим собщение
                    PrintStatus(filtredWords, excluded, required, positionExcept);
                    break;
                case UserCommand.command.position:                  // !!!!!!!!!!!!!!!!!!!!!!!!!!!
                    char letter = uc.letter;        // исключаемая буква
                    int position = uc.position;     // её позиция

                    // отсавляем только слова этой букваой необходимым буквам
                    RequiredLetters(ref filtredWords, new char[] { letter });

                    // фильтруем по позициям
                    ReqiredPosition(ref filtredWords, letter, position);

                    //RequiredLetters(ref filtredWords, letter, position);  // !!!!!!!!!!!!!!
                    positionExcept = true;
                    // добавляем отфильтрованнyю букву в общий список
                    AddAndSortLetters(ref required, new char[] { letter });
                    // Выводим собщение
                    PrintStatus(filtredWords, excluded, required, positionExcept);
                    break;
                case UserCommand.command.uncknown:
                    // комманда не распознана
                    Console.WriteLine("Команда не распознана. Повторите.");
                    break;
            }

            Console.Write("Введите комманду: ");
            userText = Console.ReadLine()!.Trim();

            //command com = GetCommand(userText);
            uc.GetCommand(userText);

            //Console.WriteLine(com.ToString());
        }// while (!exit);
    }

    private static bool AckUser(string questinon)
    {
        Console.Write(questinon);
        string ansver = Console.ReadLine()!.Trim().ToUpper();
        if (ansver == "Y") return true;
        else return false;
    }

    // Печать статуса (всего слов, исключенные и необходимые символы)
    private static void PrintStatus(string[] words, char[] excluded, char[] reqiured, bool posExcept)
    {
        string exclMsg = string.Empty;
        if (excluded.Length == 0) exclMsg = "Исключенных символов нет";
        else exclMsg = "Исключенные символы: " + CharListToString(excluded);

        string reqiurMsg = string.Empty;
        if (reqiured.Length == 0) reqiurMsg = "Обязательных символов нет";
        else reqiurMsg = "Обязательные символы: " + CharListToString(reqiured);

        string posExceptMsg = string.Empty;
        if (posExcept) posExceptMsg = " Есть исключения по позициям.";
        Console.WriteLine("Выбрано {0} слов. {1}. {2}.{3}", words.Length, exclMsg, reqiurMsg, posExceptMsg);
    }



    // получаем тип комманды из строки веденной пользователем
    private static command GetCommand(string input)
    {
        input = input.Trim().ToLower();

        if (input == "help" || input == "пом") return command.help;
        else if (input == "reset" || input == "сброс") return command.reset;
        else if (input.IndexOf("excl") == 0 || input.IndexOf("искл") == 0) return command.exclude;
        else if (input.IndexOf("req") == 0 || input.IndexOf("надо") == 0) return command.require;
        else if (input.IndexOf("pos") == 0 || input.IndexOf("поз") == 0)
        {
            // проверить формат комманды
            // должна быть буква и цифра (от 1 до 5)
            return CheckCommandFormat(input, command.position);
        }
        else if (input == "show" || input == "показ") return command.show;
        else if (input == "rand" || input == "случ") return command.random;
        else if (input == "qu" || input == "вых") return command.quit;
        //Console.WriteLine(input);
        return command.uncknown;
    }

    private static command CheckCommandFormat(string input, command cmd)            // !!!!!!!!!!!!!!
    {
        //return command.position;

        return command.uncknown;
    }

    // Извлекаем символы из комманды поользователя
    /*    private static char[] GetParametres(string input)               /// !!!!!!!!!!!!!!!!!!!!!
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
    */
    //private static char[] GetParametres(string input, command cmd)      // !!!!!!!!!!!!!!!!!!!!!!!
    /*    private static (char, int, bool) GetLetterPosParam(string input)       // !!!!!!!!!!!!!!!!!!!!!!!
        {


            // 
            char letter = 'Ё';
            int position = 0;
            bool need = true;

            if (input.IndexOf("no") > 0 || input.IndexOf("не") > 0)
            {
                int pos = input.IndexOf("не");
                if (pos < 0) pos = input.IndexOf("no");
                input = input.Remove(0, pos + 2).Trim();
                need = false;
            }
            else input = input.Remove(0, 3).Trim();
            Console.WriteLine(input);

            // // берем первую букву после комманды
            // input = input.Remove(0, 3).Trim().Remove(0, 2).Trim();
            // resChar = input[0];
            // // и первую цифру после буквы
            // foreach (char item in input)
            // {
            //     if(item >= 1 && item <=5)
            //     {
            //         resInt = int.Parse(new string(item,1));
            //         break;
            //     }
            // }


            return (letter, position, need);
        }
    */

    // Печатаем хелп
    private static void PrintHelp()
    {
        Console.WriteLine(HELPTEXT);
    }


    // исключаем по позициям
    private static void ReqiredPosition(ref string[] words, char letter, int position)
    {
        string[] tempWords = new string[words.Length];
        int len = 0;

        foreach (string word in words)
        {
            int pos = position > 0 ? position - 1 : -1 - position;
            if (position > 0 && word[pos] == letter || 
                position < 0 && word[pos] != letter)
            {
                    tempWords[len] = word;
                    len++;
            }
        }

        string[] filtred = new string[len];
        for (int i = 0; i < len; i++) filtred[i] = tempWords[i];
        words = filtred;
    }

    // выбираем только слова содержащие указанные буквы
    private static void RequiredLetters(ref string[] words, char[] required)
    {
        string[] tempWords = new string[words.Length];
        int len = 0;
        foreach (string word in words)
        {

            if (AllCharsExist(word, required))
            {
                tempWords[len] = word;
                len++;
            }
        }

        string[] filtred = new string[len];
        for (int i = 0; i < len; i++)
        {
            filtred[i] = tempWords[i];
        }
        words = filtred;
    }


    // Проверяем на наличие всех букв в слове
    private static bool AllCharsExist(string word, char[] letters)
    {
        foreach (char ch in letters)
        {
            if (word.IndexOf(ch) < 0)
                return false;
        }
        return true;
    }

    // Нормальный список из букв
    private static string CharListToString(char[] symbols)
    {
        string res = symbols[0].ToString();
        for (int i = 1; i < symbols.Length; i++)
            res += ", " + symbols[i];
        return res;
    }


    // добавляем буквы к списку
    private static void AddAndSortLetters(ref char[] exist, char[] added)
    {
        char[] newList = new char[exist.Length + added.Length];
        exist.CopyTo(newList, 0);
        added.CopyTo(newList, exist.Length);
        //  надо исключить дубликаты        !!!!!!!!!!!!!!!!!
        Array.Sort(newList);
        exist = newList;
    }

    // печатаем список слов
    private static void PrintWords(string[] words)
    {
        // Console.WriteLine("Начальный массив: [" + string.Join(", ", array) + "]");

        int n = 0;
        foreach (string w in words)
        {
            if (n % 8 == 0) Console.WriteLine();
            Console.Write(w + " \t ");
            n++;
        }
        Console.WriteLine();

    }

    // выбираем слова без указанных букв
    private static void ExcludeLetters(ref string[] words, char[] excluding)
    {
        string[] tempWords = new string[words.Length];
        int len = 0;
        foreach (string word in words)
        {
            if (!CharsExist(word, excluding))
            {
                tempWords[len] = word;
                len++;
            }
        }

        string[] filtred = new string[len];
        for (int i = 0; i < len; i++)
        {
            filtred[i] = tempWords[i];
        }
        words = filtred;

        // !!! вернуть список исключенных букв
        //return new char[0];
    }

    // есть ли буквы в слове
    private static bool CharsExist(string word, char[] letters)
    {

        foreach (char ch in letters)
        {
            if (word.IndexOf(ch) >= 0) return true;
        }

        return false;
    }



    // выбираем случайное слово из массива
    private static string GetRandonWord(string[] words, int count)
    {
        string result = string.Empty;
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                result += words[new Random().Next(words.Length)] + " \t ";
            }
        }
        return result;
    }

    // делаем строковый массив из слов(строк) в файле
    private static string[] ReadAllWords(string fileName)
    {
        int len = CountLines(fileName);
        //Console.WriteLine("Слов: " + len);
        string[] allWords = new string[len];
        StreamReader sr = new StreamReader(fileName);
        string word = sr.ReadLine()!;
        int i = 0;
        while (word != null)
        {
            allWords[i] = word;
            i++;
            word = sr.ReadLine()!;
        }
        sr.Close();
        return allWords;
    }


    // подсчёт количества строк (слов) в файле
    private static int CountLines(string fileName)
    {
        int count = 0;
        StreamReader sr = new StreamReader(fileName);
        string word = sr.ReadLine()!;
        while (word != null)
        {
            count++;
            word = sr.ReadLine()!;
        }
        sr.Close();
        return count;
    }

    //все файлики с 5ти буквенными словами собираем в 1
    private static void AllToOneFile()
    {
        string[] files = new string[] { "5а.txt", "5б.txt", "5в.txt", "5г.txt", "5д.txt", "5е.txt", "5ж.txt", "5з.txt", "5и.txt", "5к.txt", "5л.txt", "5м.txt", "5н.txt", "5о.txt", "5п.txt", "5р.txt", "5с.txt", "5т.txt", "5у.txt", "5ф.txt", "5х.txt", "5ц.txt", "5ч.txt", "5ш.txt", "5щ.txt", "5э.txt", "5ю.txt", "5я.txt" };
        string dir = Directory.GetCurrentDirectory();
        //string readFile;
        string writeFile = "5letters.txt";
        StreamReader sr;
        string word;
        int count = 0;//, dif =0;
        StreamWriter sw = new StreamWriter(dir + "/" + writeFile);
        foreach (string name in files)
        {
            sr = new StreamReader(dir + "/" + name);
            word = sr.ReadLine()!;
            while (word != null)
            {
                sw.WriteLine(word);
                word = sr.ReadLine()!;
                count++;
            }
            //Console.WriteLine(name + ": " + (count - dif));
            //dif = count;
            sr.Close();
        }
        sw.Close();
        Console.WriteLine("Всего записано {0} слов", count);
    }


    // выбираем из файла только 5ти буквенные слова и сохр их в др файл
    private static void SelectFivelettersWords(string readFile, string writeFile)
    {
        string dir = Directory.GetCurrentDirectory();
        //Console.WriteLine(dir);
        //string readFile = "букваГ.txt";
        //string writeFile = "5г.txt";
        Console.WriteLine("Читаем \"{0}\"", readFile);
        Console.WriteLine("Пишем в \"{0}\"", writeFile);
        StreamReader sr;
        StreamWriter sw;

        try
        {
            sr = new StreamReader(dir + "/" + readFile);
        }
        catch
        {
            Console.WriteLine(readFile + " не открылся");
            return;
        }
        try
        {
            sw = new StreamWriter(dir + "/" + writeFile);
        }
        catch
        {
            Console.WriteLine("Не могу создать " + writeFile);
            return;
        }

        string line = sr.ReadLine()!;
        int rLines = 0, wLines = 0;
        while (line != null)
        {
            rLines++;
            if (line.Length == 5)
            {
                //Console.WriteLine(line);
                sw.WriteLine(line);
                wLines++;
            }
            line = sr.ReadLine()!;
        }
        sr.Close();
        sw.Close();
        Console.WriteLine("Считано {0} строк. Записано {1} строк.", rLines, wLines);
    }
}
