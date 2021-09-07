﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsdzClient
{
    //[AuthorAPI(SelectableTypeDeclaration = false)]
    public interface IFfmResult : INotifyPropertyChanged
    {
        string Evaluation { get; }

        decimal ID { get; }

        string Name { get; }

        bool ReEvaluationNeeded { get; }

        bool? Result { get; }
    }
}
