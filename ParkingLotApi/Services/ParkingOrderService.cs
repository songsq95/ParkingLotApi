﻿using ParkingLotApi.Dtos;
using ParkingLotApi.Exceptions;
using ParkingLotApi.Models;
using ParkingLotApi.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingLotApi.Services
{
    public class ParkingOrderService
    {
        private readonly ParkingLotContext parkingLotContext;
        public ParkingOrderService(ParkingLotContext parkingLotContext)
        {
            this.parkingLotContext = parkingLotContext;
        }

        public async Task<int>  AddParkingOrder(ParkingOrderDto parkingOrderDto)
        {
            var foundParkingLot = parkingLotContext.ParkingLots
                .FirstOrDefault(_ => _.Name.Equals(parkingOrderDto.ParkingLotName));
            var parkingOrdersOpen = parkingLotContext.ParkingOrders.Select(_ => _.OrderStatus == true).ToList();
            if (foundParkingLot.Capacity-parkingOrdersOpen.Count > 0)
            {
                ParkingOrderEntity parkingOrderEntity = parkingOrderDto.ToEntity();
                await parkingLotContext.ParkingOrders.AddAsync(parkingOrderEntity);
                await parkingLotContext.SaveChangesAsync();
                return parkingOrderEntity.Id;
            }
            throw new NoSpaceException("The parking lot is full");
            
        }

        public async Task<ParkingOrderDto> ModifyParkingOrderStatus(ParkingOrderDto parkingOrderDto)
        {
            var foundParkingOrder = parkingLotContext.ParkingOrders
                .FirstOrDefault(_ => _.PlateNumber.Equals(parkingOrderDto.PlateNumber));
            foundParkingOrder.CloseTime = parkingOrderDto.CloseTime;
            foundParkingOrder.OrderStatus = parkingOrderDto.OrderStatus;
            return new ParkingOrderDto(foundParkingOrder);
        }
    }
}
