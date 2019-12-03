﻿using System.IO;
using System.Threading.Tasks;
using Windows.Storage;
using Zafiro.Core.Files;

namespace Zafiro.Uwp
{
    public class UwpFile : ZafiroFile
    {
        private readonly StorageFile file;

        public override string Name => file.Name;

        public UwpFile(StorageFile file)
        {
            this.file = file;
        }


        public override Task<Stream> OpenForRead()
        {
            return file.OpenStreamForReadAsync();
        }

        public override Task<Stream> OpenForWrite()
        {
            return file.OpenStreamForWriteAsync();
        }
    }
}