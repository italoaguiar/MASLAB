using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MASLAB
{
    internal static class Icons
    {
        private static IBitmap classIcon = LoadBitmap("Class.png");
        private static IBitmap propertyIcon = LoadBitmap("Property.png");
        private static IBitmap methodIcon = LoadBitmap("Method.png");
        private static IBitmap enumIcon = LoadBitmap("Enum.png");
        private static IBitmap delegateIcon = LoadBitmap("Delegate.png");
        private static IBitmap eventIcon = LoadBitmap("Event.png");
        private static IBitmap structIcon = LoadBitmap("Struct.png");
        private static IBitmap fieldIcon = LoadBitmap("Field.png");
        private static IBitmap literalIcon = LoadBitmap("Literal.png");
        private static IBitmap namespaceIcon = LoadBitmap("NameSpace.png");
        private static IBitmap interfaceIcon = LoadBitmap("Interface.png");
        private static IBitmap encapsulateFieldIcon = LoadBitmap("EncapsulateField.png");
        private static IBitmap statusErrorIcon = LoadBitmap("StatusCritialError.png");

        private static Dictionary<string, IBitmap> dic = new Dictionary<string, IBitmap>
        {
            {"Keyword", Literal },
            {"Namespace", Namespace },
            {"Class", Class },
            {"Public", Literal },
            {"Private", Literal },
            {"Internal", Literal },
            {"Protected", Literal },
            {"Static", Literal },
            {"Abstract", Literal },
            {"Structure", Struct },
            {"Delegate", Delegate },
            {"Enum", Enum },
            {"Method", Method },
            {"Property", Property },
            {"Event", Event },
            {"Local", EncapsulateField},
            {"Field", Field },
            {"Interface", Interface }
        };

        public static IBitmap Class
        { 
            get => classIcon; 
            set => classIcon = value; 
        }

        public static IBitmap Property 
        { 
            get => propertyIcon; 
            set => propertyIcon = value; 
        }

        public static IBitmap Method 
        { 
            get => methodIcon; 
            set => methodIcon = value; 
        }

        public static IBitmap Enum
        { 
            get => enumIcon; 
            set => enumIcon = value; 
        }

        public static IBitmap Delegate 
        { 
            get => delegateIcon; 
            set => delegateIcon = value; 
        }

        public static IBitmap Event
        { 
            get => eventIcon; 
            set => eventIcon = value; 
        }

        public static IBitmap Struct
        { 
            get => structIcon; 
            set => structIcon = value; 
        }

        public static IBitmap Field
        { 
            get => fieldIcon; 
            set => fieldIcon = value; 
        }

        public static IBitmap Literal
        { 
            get => literalIcon; 
            set => literalIcon = value; 
        }

        public static IBitmap Namespace 
        { 
            get => namespaceIcon; 
            set => namespaceIcon = value; 
        }

        public static IBitmap Interface 
        { 
            get => interfaceIcon; 
            set => interfaceIcon = value; 
        }
        public static IBitmap EncapsulateField { get => encapsulateFieldIcon; set => encapsulateFieldIcon = value; }
        public static IBitmap StatusErrorIcon { get => statusErrorIcon; set => statusErrorIcon = value; }

        private static IBitmap LoadBitmap(string name)
        {
            try
            {
                var assets = AvaloniaLocator.Current.GetService<IAssetLoader>();
                var bitmap = new Bitmap(assets.Open(new Uri("avares://MASLAB/Assets/" + name)));
                
                return bitmap;
            }
            catch (Exception e)
            {
                var k = e;
            }
            return null;
        }

        public static IBitmap Find(IList<string> tags)
        {
            var t = string.Join(' ', tags);

            

            foreach (var item in tags)
            {
                if (dic.ContainsKey(item)) return dic[item];                
            }

            System.Diagnostics.Debug.WriteLine(t + "\n");

            return null;
        }
    }
}