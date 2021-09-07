﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PsdzClient
{
    //[AuthorAPI(SelectableTypeDeclaration = true)]
    public interface IJob : INotifyPropertyChanged
    {
        IEnumerable<string> jobArguments { get; }

        string jobName { get; set; }

        IEnumerable<string> jobResults { get; }
    }
}
