﻿using SevenDaysProfileEditor.Data;
using SevenDaysProfileEditor.GUI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SevenDaysProfileEditor {

    internal static class Program {

        /// <summary>
        /// Initializes the static data of the program
        /// </summary>
        private static bool Initialize() {
            try {
                AssetInfo.GenerateAssetInfoList();
                IconData.itemIconDictionary = new Dictionary<string, byte[]>();
                IconData.uiIconDictionary = new Dictionary<string, UIIconData>();
                IconData.PopulateIconDictionaries();
            }
            catch (Exception e) {
                ErrorHandler.HandleError("No icons will be loaded. Failed to load asset files." + e.Message, e, true);
            }

            try {
                XmlData.GetBlocks();
                XmlData.GetItems();
                XmlData.ArrangeItemList();
                XmlData.GetSkills();
                AssetInfo.ClearAssetInfoList();
            }
            catch (Exception e) {
                ErrorHandler.HandleError(string.Format("Failed to load XML files!\n\n{0}\n\nProgram will now terminate!", e.Message), e, true);
                return false;
            }

            return true;
        }

        private static void Start(MainWindow window) {
            if (Initialize())
            {
                window.Show();
                Application.Run(window);
            }

            else
            {
                Application.Exit();
            }
        }

        [STAThread]
        private static void Main() {
            Log.startLog();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            MainWindow window = new MainWindow();
            Config.Load();

            string gameRoot = Config.GetSetting("gameRoot");

            // If no game root is specified in the config, we check if we are inside the game root. If not, we ask the user.
            if (gameRoot == null) {
                if (File.Exists(Environment.CurrentDirectory + "7DaysToDie.exe")) {
                    Start(window);
                }
                else {
                    OpenFileDialog gameRootDialog = new OpenFileDialog() {
                        Title = "Tool needs to find the 7DaysToDie.exe!",
                        Filter = "7DaysToDie.exe|7DaysToDie.exe"
                    };

                    gameRootDialog.FileOk += (sender1, e1) => {
                        gameRoot = gameRootDialog.FileName.Substring(0, gameRootDialog.FileName.LastIndexOf('\\'));
                        Config.SetSetting("gameRoot", gameRoot);
                        Start(window);
                    };

                    if (gameRootDialog.ShowDialog() != DialogResult.OK) {
                        Application.Exit();
                        return;
                    }
                }
            }
            else {
                Start(window);
            }
        }
    }
}