﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MW5.Api.Concrete;

namespace MW5.Api.Interfaces
{
    public interface IShapesList
    {
        void Add(int layerHandle, int shapeIndex);
        void Clear();
        int Count { get; }
        void RemoveByLayerHandle(int layerHandle);
        IEnumerator<SelectionItem> GetEnumerator();
    }
}
