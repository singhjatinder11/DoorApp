using MordenDoors.Database;
using MordenDoors.ViewModels;
using System;
using System.Linq;

namespace MordenDoors.Repository
{
    public class TrackingRepository
    {
        private readonly MordenDoorsEntities _context;
        public TrackingRepository()
        {
            _context = new MordenDoorsEntities();
        }
        public OrderTrackViewModel TrackOrder(string trackId)
        {
            try
            {
                OrderTrackViewModel viewModel = new OrderTrackViewModel();
                var orderDetail = (from p in _context.Orders
                                   join s in _context.OrderStatus on p.StatusId equals s.Id
                                   where p.TrackingID == trackId
                                   select new OrderTrackViewModel
                                   {
                                       Id = p.Id,
                                       OrderStatus = s.orderStatus1,
                                       TrackingID = p.TrackingID,
                                       DeliveryTime = p.DeliveryTime
                                   }).FirstOrDefault();

                if (orderDetail != null)
                {
                    viewModel.Id = orderDetail.Id;
                    //viewModel.OrderStatus = orderDetail.OrderStatus;
                    viewModel.TrackingID = orderDetail.TrackingID;
                    viewModel.DeliveryTime = orderDetail.DeliveryTime;
                    viewModel.TotalPrice = (decimal)_context.OrderItems.Where(x=>x.OrderId==orderDetail.Id).Sum(x=>x.TotalPrice);
                    viewModel.OrderItemList = (from ot in _context.OrderItems
                                               join p in _context.Products on ot.ProductId equals p.ProductId
                                               join s in _context.OrderStatus on ot.ItemStatus equals s.Id
                                               join h in _context.Orders on ot.OrderId equals h.Id
                                               where ot.OrderId == orderDetail.Id
                                               select new OrderTrackViewModel
                                               {
                                                   TotalPrice = (decimal)ot.TotalPrice,
                                                   itemStatus = orderDetail.OrderStatus,
                                                   DeliveryAddress = h.AddressLine2,
                                               }).ToList();
                };
                return viewModel;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}