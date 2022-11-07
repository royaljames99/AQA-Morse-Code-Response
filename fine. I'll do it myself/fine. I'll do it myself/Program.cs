using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace fine._I_ll_do_it_myself
{
    class Translator
    {
        //dictionaries assigning letters to their corresponding morse code strings for each standard
        Dictionary<char, string> morseDotDashDict = new Dictionary<char, string>();
        Dictionary<char, string> morseEqualsDict = new Dictionary<char, string>();

        public Translator()
        {
            loadTables();
        }

        private void loadTables()
        {
            StreamReader table = new StreamReader("convertTable.txt"); //loads table containing letter <=> morse conversions for both standards

            string line = "";
            while ((line = table.ReadLine()) != null)
            {
                string[] splitLine = line.Split(",");
                morseDotDashDict.Add(Convert.ToChar(splitLine[0].Trim()), splitLine[1].Trim()); //add dot dash format conversion to app. dict.
                morseEqualsDict.Add(Convert.ToChar(splitLine[0].Trim()), splitLine[2].Trim()); // add equals format conversion to app. dict.
            }
            table.Close();
        }

        public void textToMorse(string input, string standard)
        {
            string output = "";
            foreach(char letter in input)
            {
                if(letter == ' ') //Spaces
                {
                    if (standard == "D")
                    {
                        output += "  ";
                    }
                    else
                    {
                        output += "    ";
                    }
                }
                else if(letter == '\n') //Newlines don't change
                {
                    if (output != "")//Add leading space if needed
                    {
                        output += " ";
                    }
                    output += "\n";
                }
                else if(standard == "D") //Encode based on selected standard
                {
                    if(output != "")//Add leading space if needed
                    {
                        output += " "; 
                    }
                    //Add letter
                    output += morseDotDashDict[letter];
                }
                else
                {
                    if (output != "")//Add leading spaces if needed
                    {
                        output += "   ";
                    }
                    //Add letter
                    output += morseEqualsDict[letter];
                }
            }
            Console.WriteLine(output);
            save(output);
        }

        public void dotDashToText(string input)
        {
            string output = "";

            //Find index of next space, select text before it, translate and add to output then remove that section from the input
            int nextSpaceIndex;
            string letterSubsection = "";
            while(input.Length > 0)
            {
                //Find next space
                nextSpaceIndex = input.IndexOf(" ");
                if (nextSpaceIndex != -1) //-1 If no remaining spaces (final character)
                {
                    letterSubsection = input.Substring(0, nextSpaceIndex);

                    if (letterSubsection == "") //Space
                    {
                        input = input.Substring(nextSpaceIndex + 2);
                        output += " ";
                    }
                    else
                    {
                        input = input.Substring(nextSpaceIndex + 1);
                    }
                }
                else
                {
                    letterSubsection = input;
                    input = "";
                }

                //Check for newline
                if (letterSubsection.StartsWith("\n")) //At start
                {
                    output += "\n" + morseDotDashDict.FirstOrDefault(x => x.Value == letterSubsection.Substring(1)).Key; //Get key for corresponding value (first appearence of)
                }
                else if (letterSubsection.EndsWith("\n")) //At end
                {
                    output += morseDotDashDict.FirstOrDefault(x => x.Value == letterSubsection.Substring(0, letterSubsection.Length - 1)).Key + "\n";
                }
                else //No newline
                {
                    if (letterSubsection != "") //make sure we don't try and add a space
                    {
                        output += morseDotDashDict.FirstOrDefault(x => x.Value == letterSubsection).Key;
                    }
                }
            }

            Console.WriteLine(output);
            save(output);
        }

        public void equalsToText(string input)
        {
            string output = "";

            //Find index of next space, select text before it, translate and add to output then remove that section from the input
            int nextSpaceIndex;
            string letterSubsection = "";
            while (input.Length > 0)
            {
                //Find next space
                nextSpaceIndex = input.IndexOf("   ");
                if (nextSpaceIndex != -1) //-1 If no remaining spaces (final character)
                {
                    letterSubsection = input.Substring(0, nextSpaceIndex);

                    if (letterSubsection == "") //Space
                    {
                        input = input.Substring(nextSpaceIndex + 4);
                        output += " ";
                    }
                    else
                    {
                        input = input.Substring(nextSpaceIndex + 3);
                    }
                }
                else
                {
                    letterSubsection = input;
                    input = "";
                }

                //Check for newline
                if (letterSubsection.StartsWith("\n")) //At start
                {
                    output += "\n" + morseEqualsDict.FirstOrDefault(x => x.Value == letterSubsection.Substring(1)).Key; //Get key for corresponding value (first appearence of)
                }
                else if (letterSubsection.EndsWith("\n")) //At end
                {
                    output += morseEqualsDict.FirstOrDefault(x => x.Value == letterSubsection.Substring(0, letterSubsection.Length - 1).Trim()).Key + "\n";
                }
                else //No newline
                {
                    if (letterSubsection != "") //make sure we don't try and add a space
                    {
                        output += morseEqualsDict.FirstOrDefault(x => x.Value == letterSubsection).Key;
                    }
                }
            }

            Console.WriteLine(output);
            save(output);
        }

        private void save(string output)
        {
            bool valid = false;
            string choice = "";
            while (!valid)
            {
                Console.Write("Would you like to save (y/n): ");
                choice = Console.ReadLine().ToUpper();
                if (choice == "Y" | choice == "N")
                {
                    valid = true;
                }
                else
                {
                    Console.WriteLine("INVALID RESPONSE\n");
                }
            }

            if (choice == "Y")
            {
                //Save

                //Get filename
                bool fileSelected = false;
                string overwrite = "";
                string selectedFile = "";
                while (!fileSelected)
                {
                    Console.Write("Enter Filename (.txt will be added): ");
                    selectedFile = Console.ReadLine() + ".txt";

                    //Check if file is already in use

                    //get files
                    string directory = Directory.GetCurrentDirectory();
                    string[] files = Directory.GetFiles(directory, "*"); //Get paths of all files in local directory (\bin\Debug\netcoreapp3.1)

                    //Remove unneccessary section of filepath
                    string[] shortenedFilenames = new string[files.Length];
                    int index = 0;
                    foreach (string filename in files)
                    {
                        shortenedFilenames[index] = filename.Substring(filename.LastIndexOf("\\") + 1);
                        index++;
                    }

                    //Check for pre-existing file
                    if (shortenedFilenames.Contains(selectedFile))
                    {
                        valid = false;
                        while (!valid)
                        {
                            Console.Write("That file already exists, would you like to overwrite it (y/n)?: ");
                            overwrite = Console.ReadLine().ToUpper();
                            if (overwrite == "Y")
                            {
                                valid = true;
                                fileSelected = true;
                            }
                            else if (overwrite == "N")
                            {
                                valid = true;
                            }
                            else
                            {
                                Console.WriteLine("INVALID RESPONSE\n");
                            }
                        }
                    }
                    else
                    {
                        fileSelected = true;
                    }
                }
                //Save to file
                StreamWriter writer = new StreamWriter(selectedFile);
                writer.Write(output);
                writer.Close();
                Console.WriteLine("SAVED\n");
            }

        }
    }

    class Program
    {
        static int getChoice() //Basically the menu
        {
            bool valid = false;
            int choice = -1;
            while (!valid)
            {
                Console.WriteLine("WELCOME TO THE \"AQA IS AWFUL\" MORSE CODE TRANSLATOR");
                Console.WriteLine("1) Input text to morse");
                Console.WriteLine("2) File text to morse");
                Console.WriteLine("3) Input morse to text");
                Console.WriteLine("4) File morse to text");
                Console.WriteLine("5) Exit");
                Console.Write("Enter number of choice: ");
                try
                {
                    choice = Convert.ToInt32(Console.ReadLine());
                    if (choice > 0 && choice < 6) //check it's actually on the menu (why does it sound like this is a restaurant???)
                    {
                        valid = true;
                    }
                    else
                    {
                        Console.WriteLine("INVALID OPTION");
                    }
                }
                catch
                {
                    Console.WriteLine("INVALID OPTION");
                }
                Console.WriteLine();
            }
            return choice;
        }

        static string getStandard() //Determine which standard is being used (dotDash or equals)
        {
            string standard = "";
            bool valid = false;

            while (!valid)
            {
                Console.Write("What standard would you like to use for the morse code\ndotDash or equals (d/e): ");
                standard = Console.ReadLine().ToUpper();
                if (standard == "D" | standard == "E")
                {
                    valid = true;
                }
                else
                {
                    Console.WriteLine("INVALID CHOICE");
                }
                Console.WriteLine();
            }

            return standard;
        }

        static string loadFile()
        {
            string input = "";
            string selectedFile = "";
            bool fileSelected = false;

            while (!fileSelected)
            {
                string directory = Directory.GetCurrentDirectory();
                string[] files = Directory.GetFiles(directory, "*"); //Get paths of all files in local directory (\bin\Debug\netcoreapp3.1)

                //Remove unneccessary section of filepath
                string[] shortenedFilenames = new string[files.Length];
                int index = 0;
                foreach (string filename in files)
                {
                    shortenedFilenames[index] = filename.Substring(filename.LastIndexOf("\\") + 1);
                    index++;
                }

                //Get input from user
                Console.Write("Enter filename: ");
                string inputtedFilename = Console.ReadLine();

                //Search through files in directory and add to new array
                string[] searchedFilenames = new string[files.Length];
                index = 0;
                foreach (string filename in shortenedFilenames)
                {
                    if (filename.ToLower().Contains(inputtedFilename))
                    {
                        searchedFilenames[index] = filename;
                        index++;
                    }
                }

                //Choose file from search results
                bool validChoice = false;
                while (!validChoice)
                {
                    //Display search results
                    Console.WriteLine("SEARCH RESULTS:\n0) Re-search");
                    index = 1;
                    foreach (string filename in searchedFilenames)
                    {
                        if (filename != null)
                        {
                            Console.WriteLine($"{index}) {filename}");
                            index++;
                        }
                    }
                    //Pick file
                    Console.Write("Enter Choice: ");
                    int choice = Convert.ToInt32(Console.ReadLine());
                    if (choice > -1 && choice < index)
                    {
                        validChoice = true;
                    }
                    else
                    {
                        Console.WriteLine("INVALID CHOICE\n");
                    }

                    if (choice != 0) //If it is 0 then fileSelected remains false and this whole func repeats from the top while
                    {
                        fileSelected = true;
                        selectedFile = searchedFilenames[choice - 1];
                    }
                }
            }
            //Load text from file
            StreamReader reader = new StreamReader(selectedFile);
            string line = "";
            while ((line = reader.ReadLine()) != null)
            {
                input += line + "\n";
            }
            reader.Close();
            return input;
        } //Load text from file (used for both text and morse)

        static void enterText(Translator translator, string standard) //Get text to encode from user via commandline
        {
            Console.Write("Enter text to be encoded: ");
            string input = Console.ReadLine().ToUpper();
            translator.textToMorse(input, standard);
        }

        static void enterMorse(Translator translator, string standard) //Get morse to decode from user via commandline
        {
            Console.Write("Enter text to be encoded: ");
            string input = Console.ReadLine().ToUpper();
            if (standard == "D")
            {
                translator.dotDashToText(input);
            }
            else
            {
                translator.equalsToText(input);
            }
        }

        static void loadMorse(Translator translator, string standard) //Get more to decode from user via file
        {
            string input = loadFile();
            if (standard == "D")
            {
                translator.dotDashToText(input);
            }
            else
            {
                translator.equalsToText(input);
            }
        }

        static void Main(string[] args)
        {
            Translator translator = new Translator();

            bool running = true;
            string standard = "";  
            while (running)
            {

                int choice = getChoice(); //The menu
                
                if (choice != 5) //Don't ask for standard if the program is closing
                {
                    standard = getStandard(); //Which morse code standard are we using
                }

                //Execute
                if (choice == 1)
                {
                    enterText(translator, standard);
                }
                else if (choice == 2)
                {
                    translator.textToMorse(loadFile().ToUpper(), standard);
                }
                else if (choice == 3)
                {
                    enterMorse(translator, standard);
                }
                else if (choice == 4)
                {
                    loadMorse(translator, standard);
                }
                else if (choice == 5)
                {
                    running = false;
                }
                else
                {
                    Console.WriteLine("If you got here, I'm impressed and something broke");
                }
            }
        }
    }
}
