﻿using Prism.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProyectoGrado.Events
{
    public class ProductSelectedEvent : PubSubEvent<DataRowView>
    {
    }
}
