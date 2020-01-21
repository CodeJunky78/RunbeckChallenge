using System;
using System.IO;
using System.Net;

namespace ConsoleApp1
{
    public class FileReader
    {
        private string FileLocation;
        private string FileDirectory;
        private string FileFormat;
        private string delimiter;
        private int FieldsPerRecord;

        private bool ValidRecordFileCreated;
        private bool InvalidRecordFileCreated;

        private string ValidRecordFileName = "ValidRecords.txt";
        private string InvalidRecordFileName = "InvalidRecords.txt";
        private string validFilePath;
        private string invalidFilePath;
        
        public void processFile()
        {
            GetFile();
            GetFileFormat();
            GetFieldsPerRecord();
            SortRecords();
        }    

        private void GetFile()
        {
            bool fileFound = false;
            while (!fileFound)
            {
                // requires full file path.  i.e. C:\Users\YourUserName\projects\ThisProject\FileReader\SplitCSV.txt
                Console.WriteLine("Where is the file located?");
                FileLocation = Console.ReadLine();

                if (File.Exists(FileLocation))
                {
                    fileFound = true;
                    FileDirectory = Path.GetDirectoryName(FileLocation);
                    Console.WriteLine("Directory: " + FileDirectory);
                }
                else
                {
                    Console.WriteLine("File was not found. Please try again.");
                }
            }
        }

        private void GetFileFormat()
        {
            bool validFormat = false;

            while (!validFormat)
            {
                Console.WriteLine("Is the file format CSV (comma-separated values) or TSV (tab-separated values)?");

                FileFormat = Console.ReadLine();

                if (String.Compare(FileFormat, "CSV", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Console.WriteLine("File Format: CSV");
                    delimiter = ",";
                    validFormat = true;
                }
                else if (String.Compare(FileFormat, "TSV", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    Console.WriteLine("File Format: TSV");
                    validFormat = true;
                    delimiter = "\t";
                }
                else
                {
                    Console.WriteLine("Invalid file format.  Please try again.");
                }
            }
        }

        private void GetFieldsPerRecord()
        {
            bool validEntry = false;

            while (!validEntry)
            {
                Console.WriteLine("How many fields should each record contain?");
                try
                {
                    FieldsPerRecord = Convert.ToInt16(Console.ReadLine());
                    if (FieldsPerRecord <= 0)
                    {
                        throw new FormatException();
                    }
                    validEntry = true;
                }
                catch (FormatException fe)
                {
                    Console.WriteLine("Invalid entry. Positive integers only.");
                }
                catch (OverflowException overflowException)
                {
                    Console.WriteLine("Invalid entry.  Number too large.");
                }
            }
        }
        
        private void SortRecords()
        {
            StreamWriter validSw;
            StreamWriter invalidSw;
            
            bool validFileCreated = false;
            bool invalidFileCreated = false;
            
            using (StreamReader sr = new StreamReader(FileLocation))
            {
                // move past header line
                sr.ReadLine();
                
                string line;
                
                int count = 1;
                while ((line = sr.ReadLine()) != null)
                {
                    int cols = line.Split(delimiter).Length;
                    count++;

                    if (cols == FieldsPerRecord)
                    {
                        if (!validFileCreated)
                        {
                            createValidRecordsFile();
                            validFileCreated = true;
                        }
                        
                        validSw = new StreamWriter(validFilePath, append: true);
                        validSw.WriteLine(line);
                        validSw.Dispose();
                    }
                    else
                    {
                        if (!invalidFileCreated)
                        {
                            createInvalidRecordsFile();
                            invalidFileCreated = true;
                        } 
                        
                        invalidSw = new StreamWriter(invalidFilePath, append: true);
                        invalidSw.WriteLine(line);
                        invalidSw.Dispose();
                    }
                }
                sr.Dispose();
            }
        }
        
        private void createValidRecordsFile()
        {
            validFilePath = FileDirectory + "\\" + ValidRecordFileName;
            
            if (File.Exists(validFilePath))
            {
                File.Delete(validFilePath);
            }
            
            try
            {
                File.Create(validFilePath).Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void createInvalidRecordsFile()
        {
            invalidFilePath = FileDirectory + "\\" + InvalidRecordFileName;
            
            if (File.Exists(invalidFilePath))
            {
                File.Delete(invalidFilePath);
            }
            try
            {
                File.Create(invalidFilePath).Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}