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
        private int newBlankBufferCount;

        public Buffer CurrentBuffer { get; set; }

        public Dictionary<String, Buffer> BufferDictionary { get; set; }

        public enum IsInBufferDictionary : int { Yes, HasSimilarName, NoSimilarName }

        public BufferOrganizer()
        {
            BufferDictionary = new Dictionary<String, Buffer>();
            newBlankBufferCount = 0;
        }

        public Buffer CreateBlankBuffer()
        {
            var buffer = new Buffer("Document " + (++newBlankBufferCount));
            AddNewBuffer(buffer);
            return buffer;
        }

        public async Task<Buffer> CreateBufferFromFile(StorageFile file)
        {
            Buffer buffer = null;
            var isInBufferDictionaryStatus = IsInDictionary(file);
            if (isInBufferDictionaryStatus != IsInBufferDictionary.Yes)
            {
                buffer = await Buffer.CreateBufferFromFileAsync(file);
                AddNewBuffer(buffer, isInBufferDictionaryStatus);
            }
            return buffer;
        }


        public void AddNewBuffer(Buffer buffer, IsInBufferDictionary status = IsInBufferDictionary.NoSimilarName)
        {
            AddBufferToDictionary(buffer, status);
            CurrentBuffer = buffer;
        }

        private void AddBufferToDictionary(Buffer buffer, IsInBufferDictionary status)
        {
            switch (status)
            {
                case IsInBufferDictionary.Yes:
                    break;
                case IsInBufferDictionary.HasSimilarName:
                    BufferDictionary.Add(buffer.Path, buffer);
                    break;
                case IsInBufferDictionary.NoSimilarName:
                    BufferDictionary.Add(buffer.Name, buffer);
                    break;
                default:
                    break;
            }
        }

        internal Buffer SwitchCurrentBuffer(string key)
        {
            if (BufferDictionary.ContainsKey(key))
            {
                CurrentBuffer = BufferDictionary[key];
                return CurrentBuffer;
            }
            return null;
        }

        private IsInBufferDictionary IsInDictionary(StorageFile file)
        {
            if (BufferDictionary.ContainsKey(file.Path))
                return IsInBufferDictionary.Yes;
            if (BufferDictionary.ContainsKey(file.Name))
            {
                var buffer = BufferDictionary[file.Name];
                if (buffer.Path.Equals(file.Path))
                    return IsInBufferDictionary.Yes;
                ChangeKey(BufferDictionary, buffer.Name, buffer.Path);
                return IsInBufferDictionary.HasSimilarName;
            }
            return IsInBufferDictionary.NoSimilarName;
        }

        private void ChangeKey<TKey, TValue>(Dictionary<TKey, TValue> dictionary, TKey oldKey, TKey newKey)
        {
            if (dictionary.ContainsKey(oldKey) && !dictionary.ContainsKey(newKey))
            {
                var value = dictionary[oldKey];
                dictionary.Remove(oldKey);
                dictionary.Add(newKey, value);
            }
        }
    }
}
