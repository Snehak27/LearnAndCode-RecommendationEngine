﻿using CafeteriaServer.DTO;
using CafeteriaServer.Service;
using Newtonsoft.Json;
using System;

namespace CafeteriaServer.Commands
{
    public class SaveFinalMenuCommand : ICommand
    {
        private readonly IChefService _chefService;

        public SaveFinalMenuCommand(IChefService chefService)
        {
            _chefService = chefService;
        }

        public async Task<string> Execute(string requestData)
        {
            var response = new ResponseMessage();

            try
            {
                var request = JsonConvert.DeserializeObject<SaveFinalMenuRequest>(requestData);
                await _chefService.SaveFinalMenuAsync(request.MealTypeMenuItems);
                response.IsSuccess = true;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.ErrorMessage = ex.Message;
            }

            return JsonConvert.SerializeObject(response);
        }
    }
}
