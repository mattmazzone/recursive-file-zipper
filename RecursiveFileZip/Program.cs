using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAPICodePack.Dialogs;
using System.IO.Compression;



public class RecursiveFileSearch
{
    //Log exceptions
    static System.Collections.Specialized.StringCollection log = new System.Collections.Specialized.StringCollection();
    static List<FileInfo> htmlList = new List<FileInfo>(); 
    

    [STAThread]
    public static void Main()
    {
        //Prompt user
        Console.WriteLine("Recursive Html File Zipper - Matteo Mazzone");
        Console.WriteLine("-------------------------------------------- \n");
        Console.WriteLine("Select the folder for invididual file zip \nPress any key...");
        Console.ReadKey();
        Console.WriteLine();

        CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        dialog.InitialDirectory = "C:\\Users";
        dialog.IsFolderPicker = true;

        //Exception handling for folder selection
        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
        {
            Console.WriteLine("You selected: " + dialog.FileName);

            Console.WriteLine("Would you like to zip all .html files in this folder? Y/N");
            String input = Console.ReadLine();

            if (input == "Y" || input == "y")
            {
                //Convert the path string to DirectoryInfo type and pass to function
                DirectoryInfo selectedFolder = new DirectoryInfo(dialog.FileName);
                WalkDirectoryTree(selectedFolder);

                Console.WriteLine("\n" + CreateZipFiles() + " zip files were created. Press any key to exit...");
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
        else
        {
            Console.WriteLine("A folder was not selected. Press any key to exit...");
            Console.ReadKey();
        }

        



    }

    static void WalkDirectoryTree(System.IO.DirectoryInfo root)
    {
        System.IO.FileInfo[] files = null;
        System.IO.DirectoryInfo[] subDirs = null;

        // First, process all the files directly under this folder
        try
        {
            files = root.GetFiles("*.*");
        }
        // Requires higher privileges 
        catch (UnauthorizedAccessException e)
        {
            // This code writes out the message and continues to recurse.
            log.Add(e.Message);
        }

        catch (System.IO.DirectoryNotFoundException e)
        {
            Console.WriteLine(e.Message);
        }

        if (files != null)
        {
            foreach (System.IO.FileInfo fi in files)
            {
              
                if (fi.Extension == ".html")
                {
                    htmlList.Add(fi);
                }
                
                
                //Console.WriteLine(fi.FullName);

               
            }

            // Now find all the subdirectories under this directory.
            subDirs = root.GetDirectories();

            foreach (System.IO.DirectoryInfo dirInfo in subDirs)
            {
                // Resursive call for each subdirectory.
                WalkDirectoryTree(dirInfo);
            }
        }
    }

    static int CreateZipFiles()
    {
        int counter = 0;

        foreach (FileInfo html in htmlList)
        {
            Console.WriteLine(html.FullName);

            using (FileStream fs = new FileStream(html.DirectoryName + @"\" + Path.GetFileNameWithoutExtension(html.FullName) + ".zip", FileMode.Create))
            using (ZipArchive arch = new ZipArchive(fs, ZipArchiveMode.Create))
            {
                arch.CreateEntryFromFile(html.FullName, html.Name);


                counter++;
            }


        }
        return counter;
    }
}