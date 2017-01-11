using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;


namespace UWPCode.Models
{
    public class Buffer
    {
        private string name;
        private string text;
        private bool isSaved;
        private bool isInFileSystem;

        public Buffer()
        {
            name = "";
            text = "";
            isSaved = false;
            isInFileSystem = false;
        }


        public static async Task<Buffer> CreateBufferFromFileAsync(StorageFile file)
        {
            var buffer = new Buffer
            {
                Name = file.Name,
                Text = await FileIO.ReadTextAsync(file),
                IsSaved = true,
                IsInFileSystem = true
            };
            return buffer;
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Text
        {
            get
            {
                return text;
            }

            set
            {
                text = value;
            }
        }

        public bool IsSaved
        {
            get
            {
                return isSaved;
            }

            set
            {
                isSaved = value;
            }
        }

        public bool IsInFileSystem
        {
            get
            {
                return isInFileSystem;
            }

            set
            {
                isInFileSystem = value;
            }
        }

        public void UpdateText()
        {

        }
    }
}
