using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
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

namespace TypeaheadAIWin.Source.Views
{
    public partial class Message : ObservableObject
    {
        [ObservableProperty]
        private string _text;

        [ObservableProperty]
        private ImageSource _image;
    }

    /// <summary>
    /// Interaction logic for ChatMessageEditor.xaml
    /// </summary>
    public partial class ChatMessageEditor : UserControl
    {
        public ChatMessageEditor()
        {
            InitializeComponent();
        }

        public Message Message
        {
            get { return (Message)GetValue(ChatMessageProperty); }
            set { SetValue(ChatMessageProperty, value); }
        }

        public static readonly DependencyProperty ChatMessageProperty =
            DependencyProperty.Register("Message", typeof(Message), typeof(ChatMessageEditor), new PropertyMetadata(null, OnChatMessagePropertyChanged));

        private static void OnChatMessagePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ChatMessageEditor;
            control?.UpdateRichTextBox();
        }

        private void UpdateRichTextBox()
        {
            RichTextEditor.Document.Blocks.Clear();

            if (Message != null)
            {
                if (Message.Image != null)
                {
                    var image = new Image
                    {
                        Source = Message.Image,
                        Width = 100, // Adjust size as needed
                        Height = 100
                    };
                    var container = new InlineUIContainer(image);
                    var paragraph = new Paragraph(container);
                    RichTextEditor.Document.Blocks.Add(paragraph);
                }

                if (!string.IsNullOrEmpty(Message.Text))
                {
                    RichTextEditor.AppendText(Message.Text);
                }
            }
        }
    }
}
