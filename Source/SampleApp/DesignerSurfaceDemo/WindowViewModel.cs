using System;
using System.Collections.ObjectModel;

namespace SampleApp.DesignerSurfaceDemo
{
    public class WindowViewModel
    {
        public WindowViewModel()
        {
            Objects = new ObservableCollection<Item>()
            {
                new Text
                {
                    Left = 10,
                    Top = 20,
                    Width = 200,
                    Height = 40,
                    Value = "Here I go again!",
                },
                new Picture
                {
                    Source = new Uri("/Assets/mario.png", UriKind.Relative),
                    Left = 400,
                    Top = 200,
                    Width = 300,
                    Height = 340,
                },
                new Picture
                {
                    Source = new Uri("/Assets/block.png", UriKind.Relative),
                    Left = 400,
                    Top = 100,
                    Width = 100,
                    Height = 100,
                },
                new Picture
                {
                    Source = new Uri("/Assets/lakitu.png", UriKind.Relative),
                    Left = 200,
                    Top = 180,
                    Width = 200,
                    Height = 200,
                },
                new Picture
                {
                    Source = new Uri("/Assets/goomba.png", UriKind.Relative),
                    Left = 100,
                    Top = 70,
                    Width = 200,
                    Height = 200,
                }
            };
        }

        public ObservableCollection<Item> Objects { get; set; }
    }
}