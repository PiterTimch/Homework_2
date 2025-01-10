using Client.Connecting;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection.Metadata;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Client
{
    public partial class MainWindow : Window
    {
        private HttpClient _httpClient;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void connectBT_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _httpClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{this.portTB.Text}") };

                HttpResponseMessage response = await _httpClient.PostAsync("api/Galleries/upload", null); //для тесту

                if (response.StatusCode != System.Net.HttpStatusCode.NotFound)
                {
                    System.Windows.MessageBox.Show("Connected to the server!", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    throw new Exception("Invalid port");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void loadImagePathBT_Click(object sender, RoutedEventArgs e)
        {
            using (var openFileDialog = new System.Windows.Forms.OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
                openFileDialog.Title = "Select an Image File";

                if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.imagePathTB.Text = openFileDialog.FileName;
                }
            }
        }

        private async void uploadBT_Click(object sender, RoutedEventArgs e)
        {
            string imagePath = imagePathTB.Text;

            try
            {
                this.imageLabel.Source = ImageSender.GetImageFromServer(
                    await ImageSender.UploadImageAsync(imagePath, _httpClient), _httpClient
                    );
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
