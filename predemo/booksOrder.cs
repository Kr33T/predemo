//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace predemo
{
    using System;
    using System.Collections.Generic;
    
    public partial class booksOrder
    {
        public int orderId { get; set; }
        public int bookId { get; set; }
        public int quantity { get; set; }
    
        public virtual books books { get; set; }
        public virtual orders orders { get; set; }
    }
}
