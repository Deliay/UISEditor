using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UISEditor.Data
{
    public class UISError
    {
        public UISRuntimeException InnerException { get; private set; }
        public int ErrorLine { get; private set; }

        public UISError(UISRuntimeException err, int line)
        {
            this.ErrorLine = line;
            this.InnerException = err;

        }
    }

    [Serializable]
    public class UISRuntimeException : Exception
    {
        public UISRuntimeException(string msg) : base(msg) { }
    }

    [Serializable]
    public class UISUnexpectTag : UISRuntimeException
    {
        public UISUnexpectTag(string msg) : base(msg)
        {
        }
    }

    [Serializable]
    public class UISUnknownPerfabCurveException : UISRuntimeException
    {
        public UISUnknownPerfabCurveException(string name) : base(name) { }
    }

    [Serializable]
    public class UISUnsupportAnimationControllerException : UISRuntimeException
    {
        public UISUnsupportAnimationControllerException(string name) : base(name) { }
    }

    [Serializable]
    public class UISUnsupportPropertyException : UISRuntimeException
    {
        public UISUnsupportPropertyException(string name) : base(name) { }
    }

    [Serializable]
    public class UISUnsupportAnimationException : UISRuntimeException
    {
        public UISUnsupportAnimationException(string name) : base(name) { }
    }

    [Serializable]
    public class UISUnsupportAnimationRepeatException : UISRuntimeException
    {
        public UISUnsupportAnimationRepeatException(string name) : base(name) { }
    }

    [Serializable]
    public class UISTargetAnimationNotExistException : UISRuntimeException
    {
        public UISTargetAnimationNotExistException(string msg) : base(msg)
        {
        }
    }

    [Serializable]
    public class UISAnimationNameMissingException : UISRuntimeException
    {
        public UISAnimationNameMissingException(string msg) : base(msg)
        {
        }
    }

    [Serializable]
    public class UISUnsupportPerdefineElemenetException : UISRuntimeException
    {
        public UISUnsupportPerdefineElemenetException(string msg) : base(msg)
        {
        }
    }

    [Serializable]
    public class UISUnsupportFunctionalElemenetException : UISRuntimeException
    {
        public UISUnsupportFunctionalElemenetException(string msg) : base(msg)
        {
        }
    }

    [Serializable]
    public class UISInlineAnimationException : UISRuntimeException
    {
        public UISInlineAnimationException(string msg) : base(msg)
        {
        }
    }
    
}
