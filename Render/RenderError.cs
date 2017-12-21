using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Render
{
    [Serializable]
    public class RenderException : Exception
    {
        public RenderException(string msg) : base(msg) { }
    }

    [Serializable]
    public class MissingTEXPropertyException : RenderException
    {
        public MissingTEXPropertyException(string name) : base($"Missing TEX property in {name}") { }
    }

    [Serializable]
    public class MissingRenderImageException : RenderException
    {
        public MissingRenderImageException(string filename) : base($"Missing {filename}")
        {
        }
    }
}
