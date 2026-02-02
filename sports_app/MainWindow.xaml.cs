using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using System.Net.Http;
using Newtonsoft.Json;
using sports_app.User_control;
using System.Windows.Media.Animation;
using Microsoft.AspNetCore.SignalR.Client;
using MahApps.Metro.Controls;

namespace sports_app
{
    public partial class MainWindow : MahApps.Metro.Controls.MetroWindow
    {
        private List<string> articleTitles;
        private string universal_url;
        HubConnection connection;

        public MainWindow()
        {
            InitializeComponent();
            Debug.WriteLine($"sendMessageEllipse null? {sendMessageEllipse == null}");
            Debug.WriteLine($"NewOpenConnection null? {NewOpenConnection == null}");
            articleTitles = new List<string>();
            PopulateStackPanel("sports", MainPanel);

            connection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5000/chathub")
            .WithAutomaticReconnect()
            .Build();

            sendMessageEllipse.MouseLeftButtonDown += sendMessage_Click;
            NewOpenConnection.MouseLeftButtonDown += openConnection_Click;

            connection.Reconnecting += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Attempting to reconnect...";
                    Connection_status.Text = newMessage;
                });

                return Task.CompletedTask;
            };

            connection.Reconnected += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Reconnected to the server";
                    Connection_status.Text = newMessage;
                });

                return Task.CompletedTask;
            };

            connection.Closed += (sender) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    var newMessage = "Connection Closed";
                    Connection_status.Text = newMessage;
                    NewOpenConnection.IsEnabled = true;
                    sendMessageEllipse.IsEnabled = false;
                });

                return Task.CompletedTask;
            };
        }

        private async void openConnection_Click(object sender, RoutedEventArgs e)
        {
            connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    
                    var newMessage = $"{user}: {message}";
                    if (user == "WPF Client")
                    {
                        Debug.WriteLine("populateCommentBox on the Right hand side of the app");
                        populateCommentBox(message, Position.Right);
                    }
                    else
                    {
                        Debug.WriteLine("populateCommentBox on the Lefft hand side of the app");
                        populateCommentBox(message, Position.Left);
                    }
                });
            });

            try
            {
                await connection.StartAsync();
                Connection_status.Text = "Connection started!";
                NewOpenConnection.IsEnabled = true;
                sendMessageEllipse.IsEnabled = true;
            }
            catch (Exception ex)
            {
                Connection_status.Text = ex.Message;
            }
        }

        private async void sendMessage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Debug.WriteLine("Sending message...");
                await connection.InvokeAsync("SendMessage", "WPF Client", Comment_input.Text);
                Comment_input.Text = "";
                Debug.WriteLine("Message sent successfully!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error sending message: " + ex.Message);
                Connection_status.Text = ex.Message;
            }
        }




        private void PopulateStackPanel(string Category, StackPanel panel_name)
        {
            const string apiKey = "87e23dd9778fd8fa32cf1007bef612bd";
            string apiUrl = "https://gnews.io/api/v4/top-headlines?category=";
            string category = Category; // Example query, modify as needed

            string fullUrl = $"{apiUrl}{category}&lang=en&max=20&token={apiKey}";
            string jsonResponse;

            using (WebClient client = new WebClient())
            {
                try
                {
                    jsonResponse = client.DownloadString(fullUrl);
                    dynamic data = JsonConvert.DeserializeObject(jsonResponse);
                    List<dynamic> articles = data["articles"].ToObject<List<dynamic>>();

                    foreach (var article in articles)
                    {
                        string title = article["title"];
                        string description = article["description"];
                        string imageUrl = article["image"];
                        string articleUrl = article["url"];
                        string datePublished = article["publishedAt"];
                        string content = article["content"];
                        //string date_ago = GetTimeAgo(datePublished);

                        articleTitles.Add(title); // Store the title

                        news_tile userControl = new news_tile();
                        userControl.Article_title_PH = title;
                        Uri imageUri = new Uri(imageUrl, UriKind.Absolute);
                        BitmapImage imageSource = new BitmapImage(imageUri);
                        userControl.Source_image_PH = imageSource;
                        userControl.Margin = new Thickness(20);
                        userControl.Url = articleUrl;
                        userControl.Content_PH = description;
                        //userControl.pulish_date.Text = date_ago;

                        userControl.MouseLeftButtonUp += (sender, e) =>
                        {
                            universal_url = articleUrl;
                            Article_title_2.Text = userControl.Article_title_PH;
                            Article_image.Source = userControl.Source_image_PH;
                            Context_2.Text = userControl.Content_PH;
                            Storyboard transitionStoryboard = (Storyboard)FindResource("transition");
                            transitionStoryboard.Begin();

                        };

                        panel_name.Children.Add(userControl);
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions that may occur during the request or data parsing.
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = universal_url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur
                Console.WriteLine("Error opening URL: " + ex.Message);
            }

            e.Handled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Storyboard transitionStoryboard = (Storyboard)FindResource("transitionReverse");
            transitionStoryboard.Begin();
        }

        private void minimze_button_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void close_button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void maximise_button_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
            {
                WindowState = WindowState.Normal;
            }
            else
            {
                WindowState = WindowState.Maximized;
            }
        }

        enum Position
        {
            Left,
            Right
        }
        private void populateCommentBox(string Comment, Position Post)
        {
            new_comment_tile CommentTitle = new new_comment_tile();
            CommentTitle.CommentBox.Text = Comment;
            CommentTitle.Margin = new Thickness(0, 0, 0, 5);
            Comment_section.Children.Add(CommentTitle);
            if(Post == Position.Left)
            {
                CommentTitle.HorizontalAlignment  = HorizontalAlignment.Left;
                CommentTitle.CommentBorder.Background = new SolidColorBrush(Color.FromRgb(22, 21, 22));
                CommentTitle.CommentBorder.CornerRadius = new CornerRadius(0, 30, 30, 30);
                Grid.SetColumn(CommentTitle.CommentBorder, 1);
                Grid.SetColumn(CommentTitle.ProfilePicture, 0);

            }
            else
            {
                CommentTitle.HorizontalAlignment = HorizontalAlignment.Right;
                CommentTitle.CommentBorder.Background = new SolidColorBrush(Color.FromRgb(242, 135, 5));
                CommentTitle.CommentBorder.CornerRadius = new CornerRadius(30, 0, 30, 30);
                Grid.SetColumn(CommentTitle.CommentBorder, 0);
                Grid.SetColumn(CommentTitle.ProfilePicture, 1);
            }
        }

        //testing
        private void Comment_input_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
