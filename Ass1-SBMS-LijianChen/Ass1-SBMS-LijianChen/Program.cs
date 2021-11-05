using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security;
using System.Text.RegularExpressions;

namespace Ass1_SBMS_LijianChen
{
    class Program
    {
        static void Main(string[] args)
        {
            SimpleBankManagementSystem SBMS = new SimpleBankManagementSystem();

            SBMS.GetLoginCredentials(); // Get all login credentials

            if (SBMS.Login() == true)
            {
                SBMS.MainMenu(); // Navigate to the main menu if login successfully
            }

            Console.ReadKey();
        }
    }

    class SimpleBankManagementSystem
    {
        Accounts accounts = new Accounts();
        Transaction transaction = new Transaction();

        // Sender email credential
        private const string senderEmail = "*** SENDER EMAIL ADDRESS HERE - SMTP Gmail Server ***";
        private const string senderPassword = "*** SENDER EMAIL PASSWORD HERE ***";

        EmailService emailService = new EmailService(senderEmail, senderPassword);

        // Global variable
        // Dictionary to store the login credentials
        private Dictionary<string, string> loginCredentials = new Dictionary<string, string>();
        // ArrayList to store all account IDs
        private ArrayList allAccounts = new ArrayList();
        // List to store account transaction
        List<Transaction> accountTransaction = new List<Transaction>();

        /*
         * @brief Get all login credentials from the login text file
         * 
         * @return void
         */
        public void GetLoginCredentials()
        {
            // Path of the login text file
            const string loginFilePath = @"Database\Login Credentials\login.txt";

            try
            {
                // Read the login text file
                string[] loginFile = File.ReadAllLines(loginFilePath);
                // Loop over the login file and split it by the delimiter '|'
                foreach (string text in loginFile)
                {
                    string[] splittedText = text.Split('|');
                    // Add the username and password to the loginInfo dictionary
                    loginCredentials.Add(splittedText[0], splittedText[1]);
                }
            }
            catch (DirectoryNotFoundException e) // Catch directory not found exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(2); // Exit due to the system cannot find the file specified
            }
        }

        /*
         * @brief SBMS Login
         * 
         * @return bool - True if the username and password are matched
         */
        public bool Login()
        {
            // Cursor positions - username, password, message
            // Top: The current position, in rows, of the cursor
            // Left: The current position, in columns, of the cursor
            int userNameCursorTop, userNameCursorLeft,
                passwordCursorTop, passwordCursorLeft,
                messageCursorTop, messageCursorLeft;
            // Login input - username, password
            string username, password;

            do
            {
                /* Login interface */
                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\tWELCOME TO SIMPLE BANK SYSTEM\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t\tLOGIN TO START\t\t    ║");
                Console.WriteLine("\t\t║\t\t\t\t\t    ║"); // Empty line
                Console.Write("\t\t║\tUser Name: ");
                // Get the current cursor location for the username input
                userNameCursorTop = Console.CursorTop;
                userNameCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t    ║");
                Console.Write("\t\t║\tPassword: ");
                // Get the current cursor location for the password input
                passwordCursorTop = Console.CursorTop;
                passwordCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t    ║");
                Console.WriteLine("\t\t║\t\t\t\t\t    ║"); // Empty line
                Console.WriteLine("\t\t╚═══════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for username input
                Console.SetCursorPosition(userNameCursorLeft, userNameCursorTop);
                username = Console.ReadLine(); // Username input

                // Redirect the cursor location for password input
                Console.SetCursorPosition(passwordCursorLeft, passwordCursorTop);
                SecureString securePassword = passwordMasking(); // Password input
                password = new NetworkCredential(username, securePassword).Password; // Get password plain text

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check the login credential
                if (loginCredentials.ContainsKey(username) == true && password == loginCredentials[username])
                {
                    Console.WriteLine("Valid credential! Press any key to continue.");
                    Console.ReadKey();
                    break; // Break from the do-while loop if the username and password are correct
                }
                else
                {
                    Console.WriteLine("Invalid credential! Press any key to try again.");
                    Console.ReadKey();
                }
            } while (true);

            return true;
        }

        /*
         * @brief SBMS main menu
         * 
         * @return void
         */
        public void MainMenu()
        {
            // Cursor positions - user input, message
            int inputCursorTop, inputCursorLeft, messageCursorTop, messageCursorLeft;
            // User input
            string choice;
            // Variable to store the valid user input
            int validChoice;

            do
            {
                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\tWELCOME TO SIMPLE BANK SYSTEM\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t1. Create a new account\t\t    ║");
                Console.WriteLine("\t\t║\t2. Search for an account\t    ║");
                Console.WriteLine("\t\t║\t3. Deposit\t\t\t    ║");
                Console.WriteLine("\t\t║\t4. Withdraw\t\t\t    ║");
                Console.WriteLine("\t\t║\t5. A/C statement\t\t    ║");
                Console.WriteLine("\t\t║\t6. Delete account\t\t    ║");
                Console.WriteLine("\t\t║\t7. Exit\t\t\t\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.Write("\t\t║\tEnter your choice (1-7): ");
                // Get the current cursor location for the choice input
                inputCursorTop = Console.CursorTop;
                inputCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t    ║");
                Console.WriteLine("\t\t╚═══════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for user input
                Console.SetCursorPosition(inputCursorLeft, inputCursorTop);
                choice = Console.ReadLine();

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check if the user input is an integer and it is a valid input
                if (Int32.TryParse(choice, out validChoice) && (choice == "1" || choice == "2" || choice == "3" || choice == "4" || choice == "5" || choice == "6" || choice == "7"))
                {
                    break; // Break from the do-while loop if the user input is valid
                }
                else
                {
                    Console.WriteLine("Invalid input! Press any key to try again with valid input (1-7).");
                    Console.ReadKey();
                }
            } while (true);

            // Switch statement to navigate to other system features
            switch (validChoice)
            {
                case 1:
                    CreateNewAccount();
                    break;
                case 2:
                    SearchAccount();
                    break;
                case 3:
                    Deposit();
                    break;
                case 4:
                    Withdraw();
                    break;
                case 5:
                    AccountStatement();
                    break;
                case 6:
                    DeleteAccount();
                    break;
                case 7:
                    Exit(messageCursorTop, messageCursorLeft);
                    break;
                default:
                    break;
            }
        }

        /*
         * @brief SBMS feature - Create a new account
         * 
         * @return void
         */
        private void CreateNewAccount()
        {
            // Cursor positions
            int firstNameCursorTop, firstNameCursorLeft, 
                lastNameCursorTop, lastNameCursorLeft, 
                addressCursorTop, addressCursorLeft, 
                phoneCursorTop, phoneCursorLeft, 
                emailCursorTop, emailCursorLeft, 
                messageCursorTop, messageCursorLeft;
            // String variable for the name of new account file
            string newAccountFileName;
            // User input
            string userInput = "";
            // Current date and time
            DateTime currentDateTime;

            do
            {
                allAccounts = accounts.getAllAccounts(allAccounts); // Get all account IDs

                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\t\t\tCREATE A NEW ACCOUNT\t\t        ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t\t\t ENTER THE DETAILS\t\t        ║");
                Console.WriteLine("\t\t║\t\t\t\t\t\t\t        ║"); // Empty line
                Console.Write("\t\t║    First Name: ");
                // Get the current cursor location for the first name input
                firstNameCursorTop = Console.CursorTop;
                firstNameCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t\t\t        ║");
                Console.Write("\t\t║    Last Name: ");
                // Get the current cursor location for the last name input
                lastNameCursorTop = Console.CursorTop;
                lastNameCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t\t\t        ║");
                Console.Write("\t\t║    Address: ");
                // Get the current cursor location for the address input
                addressCursorTop = Console.CursorTop;
                addressCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t\t\t\t        ║");
                Console.Write("\t\t║    Phone: ");
                // Get the current cursor location for the phone input
                phoneCursorTop = Console.CursorTop;
                phoneCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t\t\t\t        ║");
                Console.Write("\t\t║    Email: ");
                // Get the current cursor location for the email input
                emailCursorTop = Console.CursorTop;
                emailCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t\t\t\t        ║");
                Console.WriteLine("\t\t╚═══════════════════════════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for first name input
                Console.SetCursorPosition(firstNameCursorLeft, firstNameCursorTop);
                accounts.FirstName = Console.ReadLine(); // First name input

                // Redirect the cursor location for last name input
                Console.SetCursorPosition(lastNameCursorLeft, lastNameCursorTop);
                accounts.LastName = Console.ReadLine(); // Last name input

                // Redirect the cursor location for address input
                Console.SetCursorPosition(addressCursorLeft, addressCursorTop);
                accounts.Address = Console.ReadLine(); // Address input

                // Redirect the cursor location for phone input
                Console.SetCursorPosition(phoneCursorLeft, phoneCursorTop);
                accounts.Phone = Console.ReadLine(); // Phone input

                // Redirect the cursor location for email input
                Console.SetCursorPosition(emailCursorLeft, emailCursorTop);
                accounts.Email = Console.ReadLine(); // Email input

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check the phone is an integer and no more than 10 digits, and email has contained "@"
                if (accounts.Phone.Length <= 10 && Int32.TryParse(accounts.Phone, out int validPhone) && accounts.Email.Contains("@"))
                {
                    // Check if the phone is starting with a 0
                    if (accounts.Phone.StartsWith("0"))
                    {
                        accounts.Phone = "0" + validPhone.ToString(); // Prepend a 0 to the beginning of the phone
                    }
                    else
                    {
                        accounts.Phone = validPhone.ToString();
                    }

                    // Generate an account ID for the new account
                    accounts.AccountID = Convert.ToInt32(allAccounts[allAccounts.Count - 1].ToString()) + 1;
                    accounts.Balance = 0.0; // Initial the account balance to 0

                    Console.Write("Is the information correct (y/n)? "); // Confirm the account information with the user

                    userInput = Console.ReadLine();

                    // Check the user input is a char
                    if (Char.TryParse(userInput, out char validUserInput))
                    {
                        if (validUserInput == 'y')
                        {
                            // Get the time when the user confirmed the account creation
                            currentDateTime = DateTime.Now;

                            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);

                            break; // Break from the do-while loop if the user claims the account details are correct
                        }
                        else if (validUserInput == 'n')
                        {
                            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                            Console.WriteLine("Press any key to re-enter account details.");
                            Console.ReadKey();
                        }
                        else
                        {
                            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                            Console.WriteLine("Invalid input! Press any key to re-enter account details.");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                        Console.WriteLine("Invalid input! Press any key to re-enter account details.");
                        Console.ReadKey();
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Phone/Email format!");

                    retryPrompt(messageCursorTop, messageCursorLeft, userInput);
                }
            } while (true);

            // Generate the path of the new account file
            newAccountFileName = accounts.AccountsDirectoryPath + accounts.AccountID + ".txt";

            if (!File.Exists(newAccountFileName))
            {
                // If the file does not exist, write the account information into the new account file
                using (StreamWriter newAccountFile = new StreamWriter(newAccountFileName))
                {
                    newAccountFile.Write("First Name|" + accounts.FirstName + "\n");
                    newAccountFile.Write("Last Name|" + accounts.LastName + "\n");
                    newAccountFile.Write("Address|" + accounts.Address + "\n");
                    newAccountFile.Write("Phone|" + accounts.Phone + "\n");
                    newAccountFile.Write("Email|" + accounts.Email + "\n");
                    newAccountFile.Write("AccountNo|" + accounts.AccountID + "\n");
                    newAccountFile.Write("Balance|" + accounts.Balance + "\n");
                    newAccountFile.Write(currentDateTime + "|Opening balance|" + 0 + "|" + 0 + "\n");
                }
            }

            // Send new account email to the account owner
            if (emailService.sendNewAccountEmail(accounts) == true)
            {
                Console.WriteLine("Account created! Details will be provided via email.");
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Account number is: {0}", accounts.AccountID);
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                MainMenu();
            }
            else // Inform the user if the email did not send
            {
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Account created! Details failed to send via email. Please contact the Simple Bank Management Team.");
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Account number is: {0}", accounts.AccountID);
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                MainMenu();
            }
        }

        /*
         * @brief SBMS feature - Search account(s)
         * 
         * @return void
         */
        private void SearchAccount()
        {
            // Cursor positions
            int accountIDCursorTop, accountIDCursorLeft, messageCursorTop, messageCursorLeft;
            // Account ID to search
            string accountID;
            // User input
            string userInput = "";

            do
            {
                allAccounts = accounts.getAllAccounts(allAccounts); // Get all account IDs

                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\t      SEARCH AN ACCOUNT\t\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t      ENTER THE DETAILS\t\t    ║");
                Console.WriteLine("\t\t║\t\t\t\t\t    ║"); // Empty line
                Console.Write("\t\t║    Account Number: ");
                // Get the current cursor location for the account ID input
                accountIDCursorTop = Console.CursorTop;
                accountIDCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t    ║");
                Console.WriteLine("\t\t╚═══════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for account ID input
                Console.SetCursorPosition(accountIDCursorLeft, accountIDCursorTop);
                accountID = Console.ReadLine(); // Account ID input

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check the user input is an integer and no more than 10 digits
                if (Int32.TryParse(accountID, out int validAccountID) && accountID.Length <= 10)
                {
                    if (allAccounts.Contains(accountID)) // Check if the input account ID is exist
                    {
                        Console.WriteLine("Account found!\n");

                        accounts = accounts.getAccountDetails(validAccountID); // Get the account details of the account ID queried

                        Console.WriteLine("\t╔═══════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("\t║\t\t        ACCOUNT DETAILS\t\t\t        ║");
                        Console.WriteLine("\t╠═══════════════════════════════════════════════════════════════╣");
                        Console.WriteLine("\t║\t\t\t\t\t\t\t        ║"); // Empty line
                        Console.WriteLine("\t║    Account No: {0}\t\t\t\t        ║", accounts.AccountID);
                        Console.WriteLine("\t║    Account Balance: ${0}\t\t\t\t        ║", accounts.Balance);
                        Console.WriteLine("\t║    First Name: {0}\t\t\t\t\t        ║", accounts.FirstName);
                        Console.WriteLine("\t║    Last Name: {0}\t\t\t\t\t        ║", accounts.LastName);
                        Console.WriteLine("\t║    Address: {0}\t\t        ║", accounts.Address);
                        Console.WriteLine("\t║    Phone: {0}\t\t\t\t\t        ║", accounts.Phone);
                        Console.WriteLine("\t║    Email: {0}\t\t\t║", accounts.Email);
                        Console.WriteLine("\t╚═══════════════════════════════════════════════════════════════╝");
                    }
                    else
                    {
                        Console.WriteLine("Account not found!");
                    }

                    checkAnotherAccountPrompt(messageCursorTop, messageCursorLeft, userInput);
                }
                else
                {
                    Console.WriteLine("Invalid input! Press any key to try again.");
                    Console.ReadKey();
                }
            }
            while (true);
        }

        /*
         * @brief SBMS feature - Deposit
         * 
         * @return void
         */
        private void Deposit()
        {
            // Cursor positions
            int accountIDCursorTop, accountIDCursorLeft, amountCursorTop, amountCursorLeft, messageCursorTop, messageCursorLeft;
            // Variable to store the valid account ID
            int validAccountID;
            // Variable to store the valid deposit amount
            double validAmount;
            // User input
            string userInput = "";
            // Account ID to search and amount to deposit
            string accountID, amount;
            // Transaction type that the user wants to do
            const string transactionType = "deposit";

            do
            {
                allAccounts = accounts.getAllAccounts(allAccounts); // Get all account IDs

                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\t\t    DEPOSIT\t\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t      ENTER THE DETAILS\t\t    ║");
                Console.WriteLine("\t\t║\t\t\t\t\t    ║"); // Empty line
                Console.Write("\t\t║    Account Number: ");
                // Get the current cursor location for the account ID input
                accountIDCursorTop = Console.CursorTop;
                accountIDCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t    ║");
                Console.Write("\t\t║    Amount: $");
                // Get the current cursor location for the amount input
                amountCursorTop = Console.CursorTop;
                amountCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t\t    ║");
                Console.WriteLine("\t\t╚═══════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for account ID input
                Console.SetCursorPosition(accountIDCursorLeft, accountIDCursorTop);
                accountID = Console.ReadLine(); // Account ID input

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check the user input is an integer and no more than 10 digits
                if (Int32.TryParse(accountID, out validAccountID) && accountID.Length <= 10)
                {
                    if (allAccounts.Contains(accountID)) // Check if the input account ID is exist
                    {
                        Console.WriteLine("Account found! Enter the deposit amount.");

                        accounts = accounts.getAccountDetails(validAccountID); // Get the account details of the account ID queried

                        // Redirect the cursor location for deposit amount input
                        Console.SetCursorPosition(amountCursorLeft, amountCursorTop);
                        amount = Console.ReadLine(); // Deposit amount input

                        Console.Write("\n\n\n\t\t"); // Generate indentation for the message

                        if (Double.TryParse(amount, out validAmount)) // Check if the amount input is a double type
                        {
                            if (validAmount > 0) 
                            {
                                break; // Break from the do-while loop to continue the deposit process if the deposit amount is valid
                            }
                            else
                            {
                                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                                Console.WriteLine("Invalid deposit request! Press any key to try again.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                            Console.WriteLine("Invalid amount input! Press any key to try again.");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Account not found!");

                        retryPrompt(messageCursorTop, messageCursorLeft, userInput);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input! Press any key to try again.");
                    Console.ReadKey();
                }
            } while (true);

            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);

            // Account deposit - transactionType: "deposit"
            accounts.transactionAction(accounts, validAccountID, validAmount, transactionType);

            Console.WriteLine("Deposit successful! Press any key to return to main menu.");
            Console.ReadKey();
            MainMenu();
        }

        /*
         * @brief SBMS feature - Withdraw
         * 
         * @return void
         */
        private void Withdraw()
        {
            // Cursor positions
            int accountIDCursorTop, accountIDCursorLeft, amountCursorTop, amountCursorLeft, messageCursorTop, messageCursorLeft;
            // Variable to store the valid account ID
            int validAccountID;
            // Variable to store the valid withdraw amount
            double validAmount;
            // User input
            string userInput = "";
            // Account ID to search and amount to deposit
            string accountID, amount;
            // Transaction type that the user wants to do
            const string transactionType = "withdraw";

            do
            {
                allAccounts = accounts.getAllAccounts(allAccounts); // Get all account IDs

                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\t\t   WITHDRAW\t\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t      ENTER THE DETAILS\t\t    ║");
                Console.WriteLine("\t\t║\t\t\t\t\t    ║"); // Empty line
                Console.Write("\t\t║    Account Number: ");
                // Get the current cursor location for the account ID input
                accountIDCursorTop = Console.CursorTop;
                accountIDCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t    ║");
                Console.Write("\t\t║    Amount: $");
                // Get the current cursor location for the amount input
                amountCursorTop = Console.CursorTop;
                amountCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t\t    ║");
                Console.WriteLine("\t\t╚═══════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for account ID input
                Console.SetCursorPosition(accountIDCursorLeft, accountIDCursorTop);
                accountID = Console.ReadLine(); // Account ID input

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check the user input is an integer and no more than 10 digits
                if (Int32.TryParse(accountID, out validAccountID) && accountID.Length <= 10)
                {
                    if (allAccounts.Contains(accountID)) // Check if the input account ID is exist
                    {
                        Console.WriteLine("Account found! Enter the withdraw amount.");

                        accounts = accounts.getAccountDetails(validAccountID); // Get the account details of the account ID queried

                        // Redirect the cursor location for withdraw amount input
                        Console.SetCursorPosition(amountCursorLeft, amountCursorTop);
                        amount = Console.ReadLine(); // Withdraw amount input

                        Console.Write("\n\n\n\t\t"); // Generate indentation for the message

                        if (Double.TryParse(amount, out validAmount)) // Check if the amount input is a double type
                        {
                            if (validAmount > 0 && validAmount <= accounts.Balance)
                            {
                                break; // Break from the do-while loop to continue the withdraw process if the withdraw amount is valid
                            }
                            else
                            {
                                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                                Console.WriteLine("Invalid withdraw request! Press any key to try again.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                            Console.WriteLine("Invalid amount input! Press any key to try again.");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Account not found!");

                        retryPrompt(messageCursorTop, messageCursorLeft, userInput);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input! Press any key to try again.");
                    Console.ReadKey();
                }
            } while (true);

            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);

            // Account withdrawal - transactionType: "withdraw"
            accounts.transactionAction(accounts, validAccountID, validAmount, transactionType);

            Console.WriteLine("Withdraw successful! Press any key to return to main menu.");
            Console.ReadKey();
            MainMenu();
        }

        /*
         * @brief SBMS feature - Display and email account statement
         * 
         * @return void
         */
        private void AccountStatement()
        {
            // Cursor positions
            int accountIDCursorTop, accountIDCursorLeft, messageCursorTop, messageCursorLeft;
            // Account ID to search
            string accountID;
            // User input
            string userInput = "";

            do
            {
                allAccounts = accounts.getAllAccounts(allAccounts); // Get all account IDs

                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\t          STATEMENT\t\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t      ENTER THE DETAILS\t\t    ║");
                Console.WriteLine("\t\t║\t\t\t\t\t    ║"); // Empty line
                Console.Write("\t\t║    Account Number: ");
                // Get the current cursor location for the account ID input
                accountIDCursorTop = Console.CursorTop;
                accountIDCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t    ║");
                Console.WriteLine("\t\t╚═══════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for account ID input
                Console.SetCursorPosition(accountIDCursorLeft, accountIDCursorTop);
                accountID = Console.ReadLine(); // Account ID input

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check the user input is an integer and no more than 10 digits
                if (Int32.TryParse(accountID, out int validAccountID) && accountID.Length <= 10)
                {
                    if (allAccounts.Contains(accountID)) // Check if the input account ID is exist
                    {
                        Console.WriteLine("Account found! The statement is displayed below.\n");

                        accounts = accounts.getAccountDetails(validAccountID); // Get the account details of the account ID queried

                        Console.WriteLine("\t╔═══════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("\t║\t\t        SIMPLE BANK SYSTEM\t\t\t║");
                        Console.WriteLine("\t╠═══════════════════════════════════════════════════════════════╣");
                        Console.WriteLine("\t║    Account Statement\t\t\t\t\t        ║");
                        Console.WriteLine("\t║\t\t\t\t\t\t\t        ║"); // Empty line
                        Console.WriteLine("\t║    Account No: {0}\t\t\t\t        ║", accounts.AccountID);
                        Console.WriteLine("\t║    Account Balance: ${0}\t\t\t\t        ║", accounts.Balance);
                        Console.WriteLine("\t║    First Name: {0}\t\t\t\t\t        ║", accounts.FirstName);
                        Console.WriteLine("\t║    Last Name: {0}\t\t\t\t\t        ║", accounts.LastName);
                        Console.WriteLine("\t║    Address: {0}\t\t\t║", accounts.Address);
                        Console.WriteLine("\t║    Phone: {0}\t\t\t\t\t        ║", accounts.Phone);
                        Console.WriteLine("\t║    Email: {0}\t\t\t║", accounts.Email);
                        Console.WriteLine("\t╚═══════════════════════════════════════════════════════════════╝" + "\n");

                        accountTransaction = transaction.getAccountTransaction(validAccountID); // Get the account transactions of the account ID queried

                        Console.WriteLine("\t╔═══════════════════════════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("\t║\t\t\t\t    SIMPLE BANK SYSTEM\t\t\t\t    ║");
                        Console.WriteLine("\t╠═══════════════════════════════════════════════════════════════════════════════════╣");
                        Console.WriteLine("\t║    Account Transactions\t\t\t\t\t\t\t    ║");
                        Console.WriteLine("\t║\t\t\t\t\t\t\t\t\t\t    ║"); // Empty line
                        Console.WriteLine("\t║\t  Date & Time\t    \t  Type\t        Amount\t\t    Balance\t    ║");
                        // Read through the accountTransaction list for transaction date and time, transaction type, transaction amount, and account balance
                        foreach (Transaction transaction in accountTransaction)
                        {
                            Console.WriteLine("\t║    {0}\t{1}  \t {2}\t\t     {3}\t    ║", transaction.TransactionDateTime, transaction.TransactionType, transaction.TransactionAmount, transaction.AccountBalance);
                        }
                        Console.WriteLine("\t╚═══════════════════════════════════════════════════════════════════════════════════╝");

                        setMessageDisplayPosition(messageCursorTop, messageCursorLeft);

                        Console.Write("Email statement (y/n)? "); // Ask if the user wants to get the account transactions via email

                        userInput = Console.ReadLine();

                        // Check the user input is a char
                        if (Char.TryParse(userInput, out char validUserInput))
                        {
                            if (validUserInput == 'y')
                            {
                                break; // Break from the do-while loop if the user wants to get the account transactions via email
                            }
                            else if (validUserInput == 'n')
                            {
                                MainMenu(); // Return to the main menu if the user does not want to get the account transactions via email
                            }
                            else
                            {
                                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                                Console.WriteLine("Invalid input! Press any key to return to main menu.");
                                Console.ReadKey();
                                MainMenu();
                            }
                        }
                        else
                        {
                            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                            Console.WriteLine("Invalid input! Press any key to return to main menu.");
                            Console.ReadKey();
                            MainMenu();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Account not found!");
                    }

                    checkAnotherAccountPrompt(messageCursorTop, messageCursorLeft, userInput);
                }
                else
                {
                    Console.WriteLine("Invalid input! Press any key to try again.");
                    Console.ReadKey();
                }
            } while (true);

            // Send account statement email to the account owner
            if (emailService.sendAccountStatementEmail(accounts, accountTransaction) == true)
            {
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Email sent successfully!");
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                MainMenu();
            }
            else // Inform the user if email did not send
            {
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Email failed to send! Please contact the Simple Bank Management Team.");
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Press any key to return to main menu.");
                Console.ReadKey();
                MainMenu();
            }
        }

        /*
         * @brief SBMS feature - Delete existed account from SBMS
         * 
         * @return void
         */
        private void DeleteAccount()
        {
            // Cursor positions
            int accountIDCursorTop, accountIDCursorLeft, messageCursorTop, messageCursorLeft;
            // Account ID to search
            string accountID;
            // String variable to store the path of the account file which is the one going to delete
            string accountFilePath;
            // User input
            string userInput = "";

            do
            {
                allAccounts = accounts.getAllAccounts(allAccounts); // Get all account IDs

                Console.Clear(); // Clean the console interface
                Console.WriteLine("\t\t╔═══════════════════════════════════════════╗");
                Console.WriteLine("\t\t║\t      DELETE AN ACCOUNT\t\t    ║");
                Console.WriteLine("\t\t╠═══════════════════════════════════════════╣");
                Console.WriteLine("\t\t║\t      ENTER THE DETAILS\t\t    ║");
                Console.WriteLine("\t\t║\t\t\t\t\t    ║"); // Empty line
                Console.Write("\t\t║    Account Number: ");
                // Get the current cursor location for the account ID input
                accountIDCursorTop = Console.CursorTop;
                accountIDCursorLeft = Console.CursorLeft;
                Console.WriteLine("\t\t\t    ║");
                Console.WriteLine("\t\t╚═══════════════════════════════════════════╝");
                Console.Write("\n\t\t"); // Generate indentation for the message
                // Get the current cursor location for the message display
                messageCursorTop = Console.CursorTop;
                messageCursorLeft = Console.CursorLeft;

                // Redirect the cursor location for account ID input
                Console.SetCursorPosition(accountIDCursorLeft, accountIDCursorTop);
                accountID = Console.ReadLine(); // Account ID input

                // Redirect the cursor location for message display
                Console.SetCursorPosition(messageCursorLeft, messageCursorTop);

                // Check the user input is an integer and no more than 10 digits
                if (Int32.TryParse(accountID, out int validAccountID) && accountID.Length <= 10)
                {
                    if (allAccounts.Contains(accountID)) // Check if the input account ID is exist
                    {
                        Console.WriteLine("Account found! Details displayed below.\n");

                        accounts = accounts.getAccountDetails(validAccountID); // Get the account details of the account ID queried

                        Console.WriteLine("\t╔═══════════════════════════════════════════════════════════════╗");
                        Console.WriteLine("\t║\t\t        ACCOUNT DETAILS\t\t\t        ║");
                        Console.WriteLine("\t╠═══════════════════════════════════════════════════════════════╣");
                        Console.WriteLine("\t║\t\t\t\t\t\t\t        ║"); // Empty line
                        Console.WriteLine("\t║    Account No: {0}\t\t\t\t        ║", accounts.AccountID);
                        Console.WriteLine("\t║    Account Balance: ${0}\t\t\t\t        ║", accounts.Balance);
                        Console.WriteLine("\t║    First Name: {0}\t\t\t\t\t        ║", accounts.FirstName);
                        Console.WriteLine("\t║    Last Name: {0}\t\t\t\t\t        ║", accounts.LastName);
                        Console.WriteLine("\t║    Address: {0}\t\t        ║", accounts.Address);
                        Console.WriteLine("\t║    Phone: {0}\t\t\t\t\t        ║", accounts.Phone);
                        Console.WriteLine("\t║    Email: {0}\t\t\t║", accounts.Email);
                        Console.WriteLine("\t╚═══════════════════════════════════════════════════════════════╝");

                        setMessageDisplayPosition(messageCursorTop, messageCursorLeft);

                        Console.Write("Delete account (y/n)? "); // Confirm the delete action with the user

                        userInput = Console.ReadLine();

                        // Check the user input is a char
                        if (Char.TryParse(userInput, out char validUserInput))
                        {
                            if (validUserInput == 'y')
                            {
                                // Generate the path of the account file that is going to delete
                                accountFilePath = accounts.AccountsDirectoryPath + validAccountID + ".txt";
                                break; // Break from the do-while loop to continue the account delete process when user permitted
                            }
                            else if (validUserInput == 'n')
                            {
                                MainMenu(); // Return to the main menu if the user does not want to delete the account
                            }
                            else
                            {
                                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                                Console.WriteLine("Invalid input! Press any key to try again.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                            Console.WriteLine("Invalid input! Press any key to try again.");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Account not found!");

                        checkAnotherAccountPrompt(messageCursorTop, messageCursorLeft, userInput);
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input! Press any key to try again.");
                    Console.ReadKey();
                }
            } while (true);

            try
            {
                // Delete account file
                File.Delete(accountFilePath);

                if (emailService.sendAccountDeleteEmail(accounts) == true)
                {
                    // Update accounts in the system
                    allAccounts.Clear();
                    allAccounts = accounts.getAllAccounts(allAccounts);

                    setMessageDisplayPosition(messageCursorTop, messageCursorLeft);

                    Console.WriteLine("Account deleted! Account details are sent via email. Press any key to return to main menu.");
                    Console.ReadKey();
                    MainMenu();
                }
                else
                {
                    // Update accounts in the system
                    allAccounts.Clear();
                    allAccounts = accounts.getAllAccounts(allAccounts);

                    setMessageDisplayPosition(messageCursorTop, messageCursorLeft);

                    Console.WriteLine("Account deleted! Account details failed to send via email. Press any key to return to main menu.");
                    Console.ReadKey();
                    MainMenu();
                }
            }
            catch (FileNotFoundException e) // Catch file not found exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(2); // Exit due to the system cannot find the file specified
            }
        }

        /*
         * @brief SBMS feature - Exit from SBMS or return to the login interface
         * 
         * @return void
         */
        private void Exit(int messageCursorTop, int messageCursorLeft)
        {
            // User input
            string userInput;

            do
            {
                Console.Write("Exit from the system (y/n)? "); // Ask if the user wants to exit from the system

                userInput = Console.ReadLine();

                // Check the user input is a char
                if (Char.TryParse(userInput, out char validUserInput))
                {
                    if (validUserInput == 'y')
                    {
                        Environment.Exit(0); // Exit from the system and terminate the program
                    }
                    else if (validUserInput == 'n')
                    {
                        // Return to the login interface
                        if (Login() == true)
                        {
                            MainMenu();
                        }
                    }
                    else
                    {
                        Console.SetCursorPosition(messageCursorLeft, messageCursorTop);
                        Console.WriteLine("Invalid input! Press any key to return to main menu.");
                        Console.ReadKey();
                        MainMenu();
                    }
                }
                else
                {
                    Console.SetCursorPosition(messageCursorLeft, messageCursorTop);
                    Console.WriteLine("Invalid input! Press any key to return to main menu.");
                    Console.ReadKey();
                    MainMenu();
                }
            } while (true);
        }

        /*
         * @brief Securely mask the password input from plain text to cypher (*)
         * 
         * @return SecureString - Secure string for user password
         */
        private static SecureString passwordMasking()
        {
            SecureString secureString = new SecureString();
            ConsoleKeyInfo consoleKeyInfo = new ConsoleKeyInfo();
            
            // String to display on the console when the password is typed in
            const string passwordMask = "*";
            // String to display on the console when the console key backspace is pressed
            const string removePasswordMask = "\b \b";

            while (true)
            {
                consoleKeyInfo = Console.ReadKey(true); // Do not display the pressed key on the console screen

                if (consoleKeyInfo.Key == ConsoleKey.Enter)
                {
                    break; // Break from the while loop if the input is the Enter key
                }
                else if (!char.IsControl(consoleKeyInfo.KeyChar)) // Do if the input is not the control character
                {
                    secureString.AppendChar(consoleKeyInfo.KeyChar); // Append the character to the secureString
                    Console.Write(passwordMask); // Display the password mask in the console interface
                }
                else if (secureString.Length >= 1 && consoleKeyInfo.Key == ConsoleKey.Backspace) 
                {
                    // Remove the last character from the securString if the secureString has at least 1 character and the backspace key is pressed
                    secureString.RemoveAt(secureString.Length - 1);
                    Console.Write(removePasswordMask); // Remove characters from the console interface
                }
            }

            return secureString;
        }

        /*
         * @brief Set the message display position to the next line of the current position
         * 
         * @param messageCursorTop - An integer stored the row position of the cursor
         * @param messageCursorLeft - An integer stored the column position of the cursor
         * @return void
         */
        private static void setMessageDisplayPosition(int messageCursorTop, int messageCursorLeft)
        {
            Console.Write("\n\t\t"); // Generate indentation for the message
            // Get the current cursor location for the message display
            messageCursorTop = Console.CursorTop;
            messageCursorLeft = Console.CursorLeft;
            // Redirect the cursor location for message display
            Console.SetCursorPosition(messageCursorLeft, messageCursorTop);
        }

        /*
         * @brief Ask if the user wants to re-enter to continue the process due to the previous invalid input
         * 
         * @param messageCursorTop - An integer stored the row position of the cursor
         * @param messageCursorLeft - An integer stored the column position of the cursor
         * @param userInput - A char to get the user input/decision
         * @return void
         */
        private void retryPrompt(int messageCursorTop, int messageCursorLeft, string userInput)
        {
            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
            Console.Write("Retry (y/n)? "); // Ask if the user wants to retry to input
            userInput = Console.ReadLine();

            // Check the user input is a char
            if (Char.TryParse(userInput, out char validUserInput))
            {
                if (validUserInput == 'y') { /* Do nothing to try again */ }
                else if (validUserInput == 'n')
                {
                    MainMenu(); // Return to the main menu if the user does not want to retry the current process
                }
                else
                {
                    setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                    Console.WriteLine("Invalid input! Press any key to try again.");
                    Console.ReadKey();
                }
            }
            else
            {
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Invalid input! Press any key to try again.");
                Console.ReadKey();
            }
        }

        /*
         * @brief Ask if the user wants to check another account
         * 
         * @param messageCursorTop - An integer stored the row position of the cursor
         * @param messageCursorLeft - An integer stored the column position of the cursor
         * @param userInput - A char to get the user input/decision
         * @return void
         */
        private void checkAnotherAccountPrompt(int messageCursorTop, int messageCursorLeft, string userInput)
        {
            setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
            Console.Write("Check another account (y/n)? "); // Ask if the user wants to search another account
            userInput = Console.ReadLine();

            // Check the user input is a char
            if (Char.TryParse(userInput, out char validUserInput))
            {
                if (validUserInput == 'y') { /* Do nothing to search another account */ }
                else if (validUserInput == 'n')
                {
                    MainMenu(); // Return to the main menu if the user does not want to search another account
                }
                else
                {
                    setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                    Console.WriteLine("Invalid input! Press any key to try again.");
                    Console.ReadKey();
                }
            }
            else
            {
                setMessageDisplayPosition(messageCursorTop, messageCursorLeft);
                Console.WriteLine("Invalid input! Press any key to try again.");
                Console.ReadKey();
            }
        }
    }

    class Accounts
    {
        // Account information
        private int accountID;
        private string firstName;
        private string lastName;
        private string address;
        private string phone;
        private string email;
        private double balance;
        // Path to the Accounts directory
        private const string accountsDirectoryPath = @"Database\Accounts\";

        public int AccountID
        {
            get { return accountID; }
            set { accountID = value; }
        }

        public string FirstName
        {
            get { return firstName; }
            set { firstName = value; }
        }

        public string LastName
        {
            get { return lastName; }
            set { lastName = value; }
        }

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        public string Phone
        {
            get { return phone; }
            set { phone = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public double Balance
        {
            get { return balance; }
            set { balance = value; }
        }

        public string AccountsDirectoryPath
        {
            get { return accountsDirectoryPath; }
        }

        /*
         * @brief Get all account from the Accounts directory
         * 
         * @param allAcounts - An ArrayList to store all account IDs
         * @return ArrayList - An ArrayList with all account IDs stored
         */
        public ArrayList getAllAccounts(ArrayList allAccounts)
        {
            try
            {
                // Go through the directory and capture the name of all account files
                foreach (string account in Directory.GetFiles(accountsDirectoryPath))
                {
                    // Get the account ID (file name without extension) and add it into the allAccounts ArrayList
                    allAccounts.Add(Path.GetFileNameWithoutExtension(account));
                }
            }
            catch (DirectoryNotFoundException e) // Catch directory not found exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(2); // Exit due to the system cannot find the file specified
            }

            return allAccounts;
        }

        /*
         * @brief Get account details based on the account ID
         * 
         * @param accountNo - An integer represents the query account ID
         * @return Accounts - An Accounts object that contains it corresponding details based on the account ID
         */
        public Accounts getAccountDetails(int accountNo)
        {
            Accounts account = new Accounts();
            // Path of the account file based on the account ID
            string accountFilePath = account.AccountsDirectoryPath + accountNo + ".txt";
            // Dictionary to temporarily store the account details of the account file
            Dictionary<string, string> accountDetails = new Dictionary<string, string>();

            try
            {
                // Read the corresponding account file
                string[] accountFile = File.ReadAllLines(accountFilePath);
                // Loop over the accountFile and split it by the delimiter '|'
                foreach (string text in accountFile)
                {
                    string[] splittedText = text.Split('|');
                    // Add the account details to the accountDetails dictionary
                    accountDetails.Add(splittedText[0], splittedText[1]);
                }
            }
            catch (FileNotFoundException e) // Catch file not found exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(2); // Exit due to the system cannot find the file specified
            }

            // Store the values in accountDetails into the account object
            account.AccountID = Convert.ToInt32(accountDetails["AccountNo"]);
            account.FirstName = accountDetails["First Name"];
            account.LastName = accountDetails["Last Name"];
            account.Address = accountDetails["Address"];
            account.Phone = accountDetails["Phone"];
            account.Email = accountDetails["Email"];
            account.Balance = Convert.ToDouble(accountDetails["Balance"]);

            return account;
        }

        /*
         * @brief Execute the deposit or withdraw action to the account
         * 
         * @param account - An Accounts object that contains the corresponding account details
         * @param accountNo - An integer represents the query account ID
         * @param amount - A double variable represents the amount to deposit or withdraw
         * @param transactionType - String to specify the transaction type which is deposit or withdraw
         * @return void
         */
        public void transactionAction(Accounts account, int accountNo, double amount, string transactionType)
        {
            // Valid line number of the account file (i.e. account details and maximum five transaction records)
            const int validLineNumber = 12;
            // The line number that where the oldest transaction would be in the account file
            const int lineToRemove = 7;
            // Path of the account file based on the account ID
            string accountFilePath = AccountsDirectoryPath + accountNo + ".txt";

            // The regular expression (pattern) of balance in the account file (e.g. Balance|123.123)
            const string balancePattern = @"\b(Balance)\b\|[0-9]*\.?[0-9]*";
            // Regular expression object for the specified regular expression
            Regex accountBalanceRgx = new Regex(balancePattern);

            // Get the current date and time
            DateTime currentDateTime = DateTime.Now;

            // Transaction record string that is going to write into the account file
            string transactionRecord = null;

            // Determine the transaction type
            if (transactionType == "deposit")
            {
                // Update the account balance - deposit
                account.Balance += amount;
                // Deposit transaction string
                transactionRecord = currentDateTime + "|Deposit|" + amount + "|" + account.Balance + "\n";
            }
            else if (transactionType == "withdraw")
            {
                // Update the account balance - deposit
                account.Balance -= amount;
                // Deposit transaction string
                transactionRecord = currentDateTime + "|Withdraw|" + amount + "|" + account.Balance + "\n";
            }

            try
            {
                // Do the following if the account file has reached five transactions
                if (File.ReadLines(accountFilePath).Count() == validLineNumber)
                {
                    // Read all lines of the account file and store them in the accoutFileLine string array
                    string[] accountFileLine = File.ReadAllLines(accountFilePath);

                    // Use StreamWriter to write the strings in accountFileLine into the accountFilePath
                    using (StreamWriter accountFileWriter = new StreamWriter(accountFilePath))
                    {
                        for (int count = 0; count < accountFileLine.Length; count++)
                        {
                            // Skip to remove the oldest transaction
                            if (count == lineToRemove)
                            {
                                continue;
                            }
                            else
                            {
                                accountFileWriter.WriteLine(accountFileLine[count]);
                            }
                        }
                    }
                }

                // Append deposit transaction to the end of the file
                File.AppendAllText(accountFilePath, transactionRecord);

                // Read through the account file
                string accountFile = File.ReadAllText(accountFilePath);
                // Find the string that matches the balancePattern and replace it with the new string to update the balance in the account file
                string newAccountFile = accountBalanceRgx.Replace(accountFile, "Balance|" + account.Balance);
                // Open the account file and overwrite it with the latest changes
                File.WriteAllText(accountFilePath, newAccountFile);
            }
            catch (FileNotFoundException e) // Catch file not found exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(2); // Exit due to the system cannot find the file specified
            }
        }
    }

    class Transaction : Accounts
    {
        // Transaction information
        private DateTime transactionDateTime;
        private string transactionType;
        private double transactionAmount;
        private double accountBalance;

        public DateTime TransactionDateTime
        {
            get { return transactionDateTime; }
            set { transactionDateTime = value; }
        }

        public string TransactionType
        {
            get { return transactionType; }
            set { transactionType = value; }
        }

        public double TransactionAmount
        {
            get { return transactionAmount; }
            set { transactionAmount = value; }
        }

        public double AccountBalance
        {
            get { return accountBalance; }
            set { accountBalance = value; }
        }

        /*
         * @brief Get the account transaction based on the account ID
         * 
         * @param accountNo - An integer represents the query account ID
         * @return List<Transaction> - A List contains the Transaction objects
         */
        public List<Transaction> getAccountTransaction(int accountNo)
        {
            List<Transaction> accountTransaction = new List<Transaction>();
            // Path of the account file based on the account ID
            string accountFilePath = AccountsDirectoryPath + accountNo + ".txt";
            // The line number that where the oldest transaction would be in the account file
            const int accountFileTransactionLine = 7;

            try
            {
                // Read through the account file
                string[] accountFile = File.ReadAllLines(accountFilePath);
                // ArrayList to store the transaction strings in the account file
                ArrayList accountFileTransaction = new ArrayList();

                // Start looping the account file from the oldest transaction of the account
                for (int count = accountFileTransactionLine; count < accountFile.Length; count++)
                {
                    accountFileTransaction.Add(accountFile[count]); // Add each transaction string into the accountFileTransaction ArrayList
                }

                // Loop over the accountFileTransaction ArrayList and split it by the delimiter '|'
                foreach (string text in accountFileTransaction)
                {
                    string[] splittedText = text.Split('|');

                    // Assign the value to different transaction components and add a new Transaction object to the accountTransaction List
                    accountTransaction.Add(new Transaction()
                    {
                        TransactionDateTime = Convert.ToDateTime(splittedText[0]),
                        TransactionType = splittedText[1],
                        TransactionAmount = Convert.ToDouble(splittedText[2]),
                        AccountBalance = Convert.ToDouble(splittedText[3]),
                    });
                }
            }
            catch (FileNotFoundException e) // Catch file not found exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();
                Environment.Exit(2); // Exit due to the system cannot find the file specified
            }

            return accountTransaction;
        }
    }

    class EmailService
    {
        // String to store the sender email address
        private string senderEmail;
        // String to store the sender email password
        private string senderPassword;
        // String with the SMTP (Simple Mail Transfer Protocol) server (GMail) stored
        private const string smtpServer = "smtp.gmail.com";
        // Integer with the SMTP server port stored
        private const int port = 587;

        // Constructor
        public EmailService(string senderEmail, string senderPassword)
        {
            SenderEmail = senderEmail;
            SenderPassword = senderPassword;
        }

        public string SenderEmail
        {
            get { return senderEmail; }
            set { senderEmail = value; }
        }

        public string SenderPassword
        {
            get { return senderPassword; }
            set { senderPassword = value; }
        }

        public string SmtpServer
        {
            get { return smtpServer; }
        }

        public int Port
        {
            get { return port; }
        }

        /*
         * @brief Send new account creation email to the account owner
         * 
         * @param accounts - An Accounts object that contains the corresponding account details
         * @return bool - True if the email is successfully sent
         */
        public bool sendNewAccountEmail(Accounts accounts)
        {
            MailAddress from = new MailAddress(SenderEmail); // MailAddress to specifiy the sender email address
            MailAddress to = new MailAddress(accounts.Email); // MailAddress to specify the receiver email address

            // Write the email addresses of the sender and receiver into the MailMessage object
            MailMessage mailMessage = new MailMessage(from, to);
            // Email subject
            mailMessage.Subject = String.Format("New bank account created - Account ID {0}", accounts.AccountID);
            // Email body
            mailMessage.Body = String.Format("Dear {0},\n\nThanks for using Simple Banking System! Please check below for your account details." +
                                                "\n\n\tAccount ID: {1}\n\tFirst Name: {2}\n\tLast Name: {3}\n\tAddress: {4}\n\tPhone: {5}\n\tEmail: {6}" +
                                                "\n\nDo not hesitate to contact us if you have any question.\n\nRegards,\nSimple Bank Management Team", 
                                                accounts.FirstName, accounts.AccountID, accounts.FirstName, accounts.LastName, accounts.Address, accounts.Phone, accounts.Email);

            // Establish a new SmtpClient object for SMTP service
            SmtpClient smtpClient = new SmtpClient(SmtpServer, Port)
            {
                // Assign the credential value and enable the SSL
                Credentials = new NetworkCredential(SenderEmail, SenderPassword),
                EnableSsl = true
            };

            try
            {
                smtpClient.Send(mailMessage); // Send the email

                return true; // Return true if the email is successfully sent
            }
            catch (SmtpException e) // Catch SMTP exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();

                return false;
            }
        }

        /*
         * @brief Send account statement email to the account owner
         * 
         * @param accounts - An Accounts object that contains the corresponding account details
         * @param accountTransactions - A List contains the Transaction objects
         * @return bool - True if the email is successfully sent
         */
        public bool sendAccountStatementEmail(Accounts accounts, List<Transaction> accountTransactions)
        {
            // Number of maximum transaction records in an account file
            const int maxTransactions = 5;
            // Counter for string array position
            int count = 0;

            // String array to store all transaction date and time in the account file
            string[] transactionDateTimeList = new string[maxTransactions];
            // String array to store all transaction type in the account file
            string[] transactionTypeList = new string[maxTransactions];
            // String array to store all transaction amount in the account file
            string[] transactionAmountList = new string[maxTransactions];
            // String array to store all account balance in the account file
            string[] accountBalanceList = new string[maxTransactions];

            // Loop over the accountTransaction List and store the value of each Transaction object into the corresponding string array
            foreach (Transaction transaction in accountTransactions)
            {
                transactionDateTimeList[count] = transaction.TransactionDateTime.ToString();
                transactionTypeList[count] = transaction.TransactionType;
                transactionAmountList[count] = transaction.TransactionAmount.ToString();
                accountBalanceList[count] = transaction.AccountBalance.ToString();

                count++; // Increase the counter to move the string array position
            }

            MailAddress from = new MailAddress(SenderEmail); // MailAddress to specifiy the sender email address
            MailAddress to = new MailAddress(accounts.Email); // MailAddress to specify the receiver email address

            // Write the email addresses of the sender and receiver into the MailMessage object
            MailMessage mailMessage = new MailMessage(from, to);
            // Email subject
            mailMessage.Subject = String.Format("Account Statement - Account ID {0}", accounts.AccountID);
            // Email body
            mailMessage.Body = String.Format("Dear {0},\n\nPlease check below for your account statement." +
                                                "\n\n\tAccount Information\n\tAccount ID: {1}\n\tFirst Name: {2}\n\tLast Name: {3}\n\tAddress: {4}\n\tPhone: {5}\n\tEmail: {6}" +
                                                "\n\n\tAccount Transactions (Five Records)\n\t{7} - {8} - Amount ${9} - Balance ${10}\n\t{11} - {12} - Amount ${13} - Balance ${14}" +
                                                "\n\t{15} - {16} - Amount ${17} - Balance ${18}\n\t{19} - {20} - Amount ${21} - Balance ${22}\n\t{23} - {24} - Amount ${25} - Balance ${26}" +
                                                "\n\nDo not hesitate to contact us if you have any question.\n\nRegards,\nSimple Bank Management Team", 
                                                accounts.FirstName, accounts.AccountID, accounts.FirstName, accounts.LastName, accounts.Address, accounts.Phone, accounts.Email, 
                                                transactionDateTimeList[0], transactionTypeList[0], transactionAmountList[0], accountBalanceList[0], 
                                                transactionDateTimeList[1], transactionTypeList[1], transactionAmountList[1], accountBalanceList[1], 
                                                transactionDateTimeList[2], transactionTypeList[2], transactionAmountList[2], accountBalanceList[2], 
                                                transactionDateTimeList[3], transactionTypeList[3], transactionAmountList[3], accountBalanceList[3], 
                                                transactionDateTimeList[4], transactionTypeList[4], transactionAmountList[4], accountBalanceList[4]);

            // Establish a new SmtpClient object for SMTP service
            SmtpClient smtpClient = new SmtpClient(SmtpServer, Port)
            {
                // Assign the credential value and enable the SSL
                Credentials = new NetworkCredential(SenderEmail, SenderPassword),
                EnableSsl = true
            };

            try
            {
                smtpClient.Send(mailMessage); // Send the email

                return true; // Return true if the email is successfully sent
            }
            catch (SmtpException e) // Catch SMTP exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();

                return false;
            }
        }

        /*
         * @brief Send account delete email to the account owner
         * 
         * @param accounts - An Accounts object that contains the corresponding account details
         * @return bool - True if the email is successfully sent
         */
        public bool sendAccountDeleteEmail(Accounts accounts)
        {
            MailAddress from = new MailAddress(SenderEmail); // MailAddress to specifiy the sender email address
            MailAddress to = new MailAddress(accounts.Email); // MailAddress to specify the receiver email address

            // Write the email addresses of the sender and receiver into the MailMessage object
            MailMessage mailMessage = new MailMessage(from, to);
            // Email subject
            mailMessage.Subject = String.Format("Account deleted - Account ID {0}", accounts.AccountID);
            // Email body
            mailMessage.Body = String.Format("Dear {0},\n\nYour account is deleted from the Simple Banking System. Please check below for your account details that are before account deletion." +
                                                "\n\n\tAccount ID: {1}\n\tBalance: ${2}\n\tFirst Name: {3}\n\tLast Name: {4}\n\tAddress: {5}\n\tPhone: {6}\n\tEmail: {7}" +
                                                "\n\nPlease contact us ASAP if you did not delete the account, otherwise please ignore this email.\n\nRegards,\nSimple Bank Management Team", 
                                                accounts.FirstName, accounts.AccountID, accounts.Balance, accounts.FirstName, accounts.LastName, accounts.Address, accounts.Phone, accounts.Email);

            // Establish a new SmtpClient object for SMTP service
            SmtpClient smtpClient = new SmtpClient(SmtpServer, Port)
            {
                // Assign the credential value and enable the SSL
                Credentials = new NetworkCredential(SenderEmail, SenderPassword),
                EnableSsl = true
            };

            try
            {
                smtpClient.Send(mailMessage); // Send the email

                return true; // Return true if the email is successfully sent
            }
            catch (SmtpException e) // Catch SMTP exception
            {
                Console.WriteLine(e.Message);
                Console.ReadKey();

                return false;
            }
        }
    }
}
