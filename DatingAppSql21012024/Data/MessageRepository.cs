﻿using AutoMapper;
using Dapper;
using DatingAppSql21012024.DTOs;
using DatingAppSql21012024.Entities;
using DatingAppSql21012024.Helpers;
using DatingAppSql21012024.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Reflection;

namespace DatingAppSql21012024.Data;

public class MessageRepository : IMessageRepository
{
    private readonly IMapper _mapper;
    private IDbConnection db;
    public MessageRepository(IConfiguration configuration, IMapper mapper)
    {
        this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        _mapper = mapper;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<bool> AddMessage(Message message)
    {
        var parameters = new DynamicParameters();

        parameters.Add("@senderId", message.SenderId);
        parameters.Add("@senderUsername", message.SenderUsername);
        parameters.Add("@recipientId", message.RecipientId);
        parameters.Add("@recipientUsername", message.RecipientUsername);
        parameters.Add("@content", message.Content);

        var succes = await db.QueryAsync<int>("sp_addMsg",
                                               parameters,
                                               commandType: CommandType.StoredProcedure);

        return succes.FirstOrDefault() == 1 ? true : false;
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<bool> DeleteMessage(Message message, string userNameDeleting)
    {
        var succes = await db.QueryAsync<int>("sp_deleteMsg",
                                    new { msgId = message.Id, userNameDeleting },
                                    commandType: CommandType.StoredProcedure);

        return succes.FirstOrDefault() > 0 ? true : false;

        // manda 0 si no existe msg,
        // 1 si solo edita y no borra(si el mismo manda a borrar, el front no deja tambien va a mandar 1 )
        // 2 si edita y borra
        // null si se manda un user q no es ni el sender ni el recipient
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<Message> GetMessage(int id)
    {
        var msg = await db.QueryAsync<Message>("sp_getMsg",
                                    new { msgId = id },
                                    commandType: CommandType.StoredProcedure);

        return msg.FirstOrDefault();
    }
    /*
    public async Task<Message> GetMessage(int id)
    {
        Message msg;
        List<Photo> photos;


        using (var lists = await db.QueryMultipleAsync("sp_getMsg",
                                    commandType: CommandType.StoredProcedure))
        {
            msg = lists.Read<Message>().FirstOrDefault();
            photos = lists.Read<Photo>().ToList();
        }

        // depende de si quiero sacar msg o msgDto
        // x ahora no ocupo foto

        return msg;

        //msg.ForEach(u =>
        //{
        //    u.Photos = photos.Where(p => p.AppUserId == u.Id)
        //                     .ToList();
        //});
    }
    */
    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<IEnumerable<MessageDto>> GetMessagesForUser(MessageParams messageParams)
    {
        List<Message> messages;
        List<Photo> photos;

        using (var lists = await db.QueryMultipleAsync("sp_getMsgsForUser",
                                    new { username = messageParams.Username, container = messageParams.Container },
                                    commandType: CommandType.StoredProcedure))
        {
            messages = lists.Read<Message>().ToList();
            photos = lists.Read<Photo>().ToList();
        }

        var messagesDto = _mapper.Map<List<MessageDto>>(messages);

        messagesDto.ForEach(m =>
        {
            m.SenderPhotoUrl = photos.Where(p => p.AppUserId == m.SenderId)
                                     .FirstOrDefault().Url;

            m.RecipientPhotoUrl = photos.Where(p => p.AppUserId == m.RecipientId)
                                     .FirstOrDefault().Url; // estoy mandando solo las main photo
        });

        return messagesDto.OrderBy(m => m.MessageSent);
    }


    ////////////////////////////////////////////////
    ///////////////////////////////////////////////////
    //
    public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUserName, string recipientUserName)
    { // sp_getMsgsThread manda lista de mensajes y fotos
        List<Message> messages;
        List<Photo> photos;

        using (var lists = await db.QueryMultipleAsync("sp_getMsgsThread",
                                    new { currentUserName, recipientUserName },
                                    commandType: CommandType.StoredProcedure))
        {
            messages = lists.Read<Message>().ToList();
            photos = lists.Read<Photo>().ToList();
        }

        var messagesDto = _mapper.Map<List<MessageDto>>(messages);

        messagesDto.ForEach(m =>
        {
            m.SenderPhotoUrl = photos.Where(p => p.AppUserId == m.SenderId)
                                     .FirstOrDefault().Url;

            m.RecipientPhotoUrl = photos.Where(p => p.AppUserId == m.RecipientId)
                                     .FirstOrDefault().Url; ;
        });

        return messagesDto.OrderBy(m => m.MessageSent);
    }
}

