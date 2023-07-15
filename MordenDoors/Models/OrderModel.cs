using MordenDoors.Database;
using System;
using System.Collections.Generic;

namespace MordenDoors.Models
{
    public class OrderModel
    {
        public List<OrderItemModel> OrderItems { get; set; }
        public List<UserItemModel> UserItems { get; set; }
        public string Orderid { get; set; }
        public enum OrderItemStatus : sbyte
        {
            Quote = 1,
            InProgress,
            Ready,
            Shipped,
            Completed
        }
    }
    public class OrderModelNew
    {
        public List<OrderItemModelList> OrderItems { get; set; }
        public List<UserItemModel> UserItems { get; set; }
        public string Orderid { get; set; }
        public enum OrderItemStatus : sbyte
        {
            Quote = 1,
            InProgress,
            Ready,
            Shipped,
            Completed
        }
    }
    public class OrderItemWorkStages
    {
        public List<ProductWorkstages> OrderItemWorkstages { get; set; }
        public int ProductId { get; set; }
    }
}