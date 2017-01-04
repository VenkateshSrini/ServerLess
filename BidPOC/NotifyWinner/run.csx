#r "Microsoft.WindowsAzure.Storage"
#r "Microsoft.Azure.WebJobs.Extensions.SendGrid"
#load "..\Entities\BidLogs.csx"
using System;
using SendGrid.Helpers.Mail;
public static void Run(BidLogs myQueueItem, TraceWriter log, out Mail mailMessage)
{
    //var bidLogs = myQueueItem;
    mailMessage = new Mail();
   
        var personalization = new Personalization();
        var receipientList = myQueueItem.BidWinnerEmails.Split(';');
        foreach (string receipient in receipientList)
            personalization.AddTo(new Email(receipient));

        var messageContent = new Content("text/html", "You have won the bid");
        mailMessage.AddContent(messageContent);
        mailMessage.AddPersonalization(personalization);

    log.Info("NotifyWinner invoked");



}