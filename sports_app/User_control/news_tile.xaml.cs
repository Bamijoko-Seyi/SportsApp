using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
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
using System.Windows.Shapes;

namespace sports_app.User_control
{
    /// <summary>
    /// Interaction logic for news_tile.xaml
    /// </summary>
    public partial class news_tile : UserControl
    {
        public news_tile()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string article_title_PH;
        public string Article_title_PH
        {
            get { return article_title_PH; }
            set
            {
                if (article_title_PH != value)
                {
                    article_title_PH = value;
                    OnPropertyChanged("Article_title_PH");
                }
            }
        }

        private string content_PH;
        public string Content_PH
        {
            get { return content_PH; }
            set
            {
                if (content_PH != value)
                {
                    content_PH = value;
                    OnPropertyChanged("Content_PH");
                }
            }
        }

        private ImageSource source_image_PH;
        public ImageSource Source_image_PH
        {
            get { return source_image_PH; }
            set
            {
                if (source_image_PH != value)
                {
                    source_image_PH = value;
                    OnPropertyChanged("Source_image_PH");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Url { get; set; } // Add a property to store the URL

        

        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = Cursors.Hand;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            Mouse.OverrideCursor = null;
        }

        public static readonly RoutedEvent TileClickedEvent =
            EventManager.RegisterRoutedEvent(
                "TileClicked",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(news_tile)
            );

        public event RoutedEventHandler TileClicked
        {
            add { AddHandler(TileClickedEvent, value); }
            remove { RemoveHandler(TileClickedEvent, value); }
        }

        // Raise the custom routed events
        private void RaiseTileClickedEvent()
        {
            RoutedEventArgs args = new RoutedEventArgs(TileClickedEvent);
            RaiseEvent(args);
        }
    }
}
