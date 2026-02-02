using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for new_comment_tile.xaml
    /// </summary>
    public partial class new_comment_tile : UserControl
    {
        public new_comment_tile()
        {
            InitializeComponent();
            DataContext = this;
        }

        private string comment_PH;
        public string Comment_PH
        {
            get { return comment_PH; }
            set
            {
                if (comment_PH != value)
                {
                    comment_PH = value;
                    OnPropertyChanged("Comment_PH");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
