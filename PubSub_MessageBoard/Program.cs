using System;
using System.Collections.Generic;

namespace PubSub_MessageBoard
{
    class Program
    {
        //Communication Notification Scheme Class, will inheret EventArguments -> (object sender, EventArgs e)
        public class CustomEventAgs : EventArgs
        {
            //The Data Contents (Inside the Envelope)
            public string message { get; set; }

            //The Envelope
            public CustomEventAgs(string note)
            {
                this.message = note;
            }
        }

        class Person
        {
            //I can handle an Envelope
            public event EventHandler<CustomEventAgs> customEvent;

            //I know who I am
            public string ID { get; set; }

            //I can send an Envelope
            public string post { get; set; }

            //My mailing list
            public HashSet<string> People;

            //This is who I am
            public Person(string id)
            {
                this.ID = id;
                People = new HashSet<string>();
            }

            //Tell mailing list that I've sent them a message
            public void notify()
            {
                //create temp customEvent
                EventHandler<CustomEventAgs> handle = customEvent;
                //check to ensure receipients have subscribbed to user, if not, than ignore
                if (handle != null)
                {
                    //Assign Message to Envelope
                    CustomEventAgs args = new CustomEventAgs(post);
                    //Call Event Method -> EventMethodName(object sender, EventArgs e)
                    handle(this, args);
                }
            }

            //I will populate the contents of the data and send it to everyone on my mailing list.
            public void WriteMessage(string message)
            {
                //captures the person who posted message and sends back half of message 
                //through the eventhandler to the person who has subscribbed
                this.post = (this.ID + " posted: " + message);
                Console.WriteLine("{0} posted: {1}", ID, message);
                //raise event
                this.notify();
            }

            //I got a notification from someone I subscribe to
            public void HandleEvent(object sender, CustomEventAgs e)
            {
                Console.WriteLine("\t{0}'s update from {1}", ID, e.message);
            }

            //I want to listen to pub's notifications
            public void subscribe(Person pub)
            {
                //Make sure you can't subscribe to another person twice
                //and you can't subscribe to yourself
                if (!People.Contains(pub.ID) && (this.ID != pub.ID))
                {
                    pub.customEvent += HandleEvent;
                    People.Add(pub.ID);
                }
            }
            //I no longer want to listen to pub's notifications
            public void unsubscribe(Person pub)
            {
                if (People.Contains(pub.ID))
                {
                    pub.customEvent -= HandleEvent;
                    People.Remove(pub.ID);
                }
            }
        }
        static void Main(string[] args)
        {
            //3 accounts
            Person Zach = new Person("Zach");
            Person Ross = new Person("Ross");
            Person Grant = new Person("Grant");

            Grant.subscribe(Ross);
            Zach.subscribe(Grant);
            //test for double subscribe
            Ross.subscribe(Zach);
            Ross.subscribe(Grant);

            Grant.WriteMessage("I'm changing my name to Alan Gigahertz");

            Zach.WriteMessage("Good Call");

            Ross.WriteMessage("It was my Idea");

        }
    }
}

