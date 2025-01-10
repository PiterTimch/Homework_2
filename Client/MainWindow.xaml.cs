using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
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

        private void connectBT_Click(object sender, RoutedEventArgs e)
        {
            _httpClient = new HttpClient { BaseAddress = new Uri($"http://localhost:{this.portTB.Text}") };
            System.Windows.MessageBox.Show("Connected to the server!");
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

            if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            {
                System.Windows.MessageBox.Show("Please select a valid image file.");
                return;
            }

            try
            {
                // Читання зображення і конвертація в Base64
                byte[] imageBytes = File.ReadAllBytes(imagePath);
                string base64Image = Convert.ToBase64String(imageBytes);

                var uploadData = new { Photo = $"data:image/jpeg;base64,{base64Image}" };
                var content = new StringContent(JsonSerializer.Serialize(uploadData), Encoding.UTF8, "application/json");

                // Відправлення зображення на сервер
                HttpResponseMessage response = await _httpClient.PostAsync("api/Galleries/upload", content);

                if (response.IsSuccessStatusCode)
                {
                    // Отримання URL завантаженого зображення
                    var responseData = await response.Content.ReadAsStringAsync();
                    var imageUrl = JsonDocument.Parse(responseData).RootElement.GetProperty("image").GetString();

                    // Відображення зображення
                    var fullUrl = new Uri(_httpClient.BaseAddress, imageUrl);
                    imageLabel.Source = new BitmapImage(fullUrl);

                    System.Windows.MessageBox.Show("Image uploaded successfully!");
                }
                else
                {
                    System.Windows.MessageBox.Show($"Error uploading image: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}
