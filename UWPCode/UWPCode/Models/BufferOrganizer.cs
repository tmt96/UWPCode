using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace UWPCode.Models
{
    class BufferOrganizer
    {
        private List<Buffer> bufferList;
        private Buffer currentBuffer;

        public BufferOrganizer()
        {
            bufferList = new List<Buffer>();
        }

        public Buffer CurrentBuffer
        {
            get
            {
                return currentBuffer;
            }

            set
            {
                currentBuffer = value;
            }
        }

        public List<Buffer> BufferList
        {
            get
            {
                return bufferList;
            }

            set
            {
                bufferList = value;
            }
        }

        public Buffer CreateBlankBuffer()
        {
            var buffer = new Buffer
            {
                Name = "Document 1"
            };
            AddNewBuffer(buffer);
            return buffer;
        }

        public async Task<Buffer> CreateBufferFromFile(StorageFile file)
        {
            var buffer = await Buffer.CreateBufferFromFileAsync(file);
            AddNewBuffer(buffer);
            return buffer;
        }


        public void AddNewBuffer(Buffer buffer)
        {
            bufferList.Add(buffer);
            currentBuffer = buffer;
        }
    }
}
