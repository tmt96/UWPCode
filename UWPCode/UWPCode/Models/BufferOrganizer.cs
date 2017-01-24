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
        private HashSet<Buffer> bufferList;
        private Buffer currentBuffer;
        private int newBlankBufferCount;

        public BufferOrganizer()
        {
            bufferList = new HashSet<Buffer>();
            newBlankBufferCount = 0;
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

        public HashSet<Buffer> BufferList
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
            var buffer = new Buffer("Document " + (++newBlankBufferCount));
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
