using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Game_team
{
    public partial class MainWindow : Window
    {
        private List<string> playersList = new List<string>();
        private readonly string filePath = "players.txt";

        public MainWindow()
        {
            InitializeComponent();
            LoadPlayersFromFile();
        }

        private void LoadPlayersFromFile()
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string[] lines = File.ReadAllLines(filePath);
                    foreach (string line in lines)
                    {
                        string name = line.Trim();
                        if (name != "" && !playersList.Contains(name))
                        {
                            playersList.Add(name);
                        }
                    }
                }
                RefreshPlayersListUI();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void SavePlayersToFile()
        {
            try
            {
                File.WriteAllLines(filePath, playersList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void RefreshPlayersListUI()
        {
            LbPlayers.ItemsSource = null;
            LbPlayers.ItemsSource = playersList;
        }

        private void BtnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddPlayer();
        }

        private void TxtPlayerName_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddPlayer();
            }
        }

        private void AddPlayer()
        {
            string name = TxtPlayerName.Text.Trim();
            if (name != "")
            {
                if (!playersList.Contains(name))
                {
                    playersList.Add(name);
                    TxtPlayerName.Clear();
                    RefreshPlayersListUI();
                    SavePlayersToFile();
                }
                else
                {
                    MessageBox.Show("هذا الاسم موجود مسبقاً!");
                }
            }
            TxtPlayerName.Focus();
        }

        private void BtnRemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (LbPlayers.SelectedItem != null)
            {
                string selectedPlayer = LbPlayers.SelectedItem.ToString();
                playersList.Remove(selectedPlayer);
                RefreshPlayersListUI();
                SavePlayersToFile();
            }
            else
            {
                MessageBox.Show("الرجاء اختيار لاعب لحذفه.");
            }
        }

        private void BtnRandomize_Click(object sender, RoutedEventArgs e)
        {
            if (playersList.Count == 0)
            {
                MessageBox.Show("قائمة اللاعبين فارغة!");
                return;
            }

            int numberOfTeams;
            bool success = int.TryParse(TxtTeamsCount.Text, out numberOfTeams);
            if (!success || numberOfTeams <= 0)
            {
                MessageBox.Show("الرجاء إدخال رقم صحيح لعدد الفرق.");
                return;
            }

            int totalPlayers = playersList.Count;
            if (numberOfTeams > totalPlayers)
            {
                numberOfTeams = totalPlayers;
            }

            List<string> tempPlayers = new List<string>(playersList);
            List<string> shuffledPlayers = new List<string>();
            Random rng = new Random();

            while (tempPlayers.Count > 0)
            {
                int index = rng.Next(tempPlayers.Count);
                shuffledPlayers.Add(tempPlayers[index]);
                tempPlayers.RemoveAt(index);
            }

            List<TeamModel> resultTeams = new List<TeamModel>();

            for (int i = 1; i <= numberOfTeams; i++)
            {
                TeamModel team = new TeamModel();
                team.TeamName = "الفريق رقم " + i;
                resultTeams.Add(team);
            }

            for (int i = 0; i < totalPlayers; i++)
            {
                int teamIndex = i % numberOfTeams;
                resultTeams[teamIndex].Players.Add(shuffledPlayers[i]);
            }

            IcTeams.ItemsSource = resultTeams;
        }
    }

    public class TeamModel
    {
        public string TeamName { get; set; }
        public List<string> Players { get; set; } = new List<string>();
    }
}