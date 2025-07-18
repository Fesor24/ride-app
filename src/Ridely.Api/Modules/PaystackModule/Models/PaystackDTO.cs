﻿namespace DotnetAuthBoilerPlate.Modules.PaystackModule.Models
{
    //public class PaystackChargeDataModel
    //{
    //    public uint id { get; set; }
    //    public string domain { get; set; }
    //    public string status { get; set; }
    //    public string reference { get; set; }
    //    public int amount { get; set; }
    //    public object message { get; set; }
    //    public string gateway_response { get; set; }
    //    public DateTime paid_at { get; set; }
    //    public DateTime created_at { get; set; }
    //    public string channel { get; set; }
    //    public string currency { get; set; }
    //    public string ip_address { get; set; }
    //    public object metadata { get; set; }
    //    public Log log { get; set; }
    //    public object fees { get; set; }
    //    public Customer customer { get; set; }
    //    public Authorization authorization { get; set; }
    //    public Plan plan { get; set; }

    //    //sub

    //    public class Authorization
    //    {
    //        public string authorization_code { get; set; }
    //        public string bin { get; set; }
    //        public string last4 { get; set; }
    //        public string exp_month { get; set; }
    //        public string exp_year { get; set; }
    //        public string card_type { get; set; }
    //        public string bank { get; set; }
    //        public string country_code { get; set; }
    //        public string brand { get; set; }
    //        public string account_name { get; set; }
    //    }

    //    public class Customer
    //    {
    //        public int id { get; set; }
    //        public string first_name { get; set; }
    //        public string last_name { get; set; }
    //        public string email { get; set; }
    //        public string customer_code { get; set; }
    //        public object phone { get; set; }
    //        public object metadata { get; set; }
    //        public string risk_action { get; set; }
    //    }

    //    public class History
    //    {
    //        public string type { get; set; }
    //        public string message { get; set; }
    //        public int time { get; set; }
    //    }

    //    public class Log
    //    {
    //        public int time_spent { get; set; }
    //        public int attempts { get; set; }
    //        public string authentication { get; set; }
    //        public int errors { get; set; }
    //        public bool success { get; set; }
    //        public bool mobile { get; set; }
    //        public List<object> input { get; set; }
    //        public object channel { get; set; }
    //        public List<History> history { get; set; }
    //    }

    //    public class Plan
    //    {
    //    }
    //}

    //public class PaystackTransferDataModel
    //{
    //    public uint amount { get; set; }
    //    public string currency { get; set; }
    //    public string domain { get; set; }
    //    public object failures { get; set; }
    //    public int id { get; set; }
    //    public Integration integration { get; set; }
    //    public string reason { get; set; }
    //    public string reference { get; set; }
    //    public string source { get; set; }
    //    public object source_details { get; set; }
    //    public string status { get; set; }
    //    public object titan_code { get; set; }
    //    public string transfer_code { get; set; }
    //    public object transferred_at { get; set; }
    //    public Recipient recipient { get; set; }
    //    public Session session { get; set; }
    //    public DateTime created_at { get; set; }
    //    public DateTime updated_at { get; set; }

    //    public class Session
    //    {
    //        public object provider { get; set; }
    //        public object id { get; set; }
    //    }

    //    public class Details
    //    {
    //        public string authorization_code { get; set; }
    //        public string account_number { get; set; }
    //        public object account_name { get; set; }
    //        public string bank_code { get; set; }
    //        public string bank_name { get; set; }
    //    }

    //    public class Integration
    //    {
    //        public int id { get; set; }
    //        public bool is_live { get; set; }
    //        public string business_name { get; set; }
    //    }

    //    public class Recipient
    //    {
    //        public bool active { get; set; }
    //        public string currency { get; set; }
    //        public string description { get; set; }
    //        public string domain { get; set; }
    //        public object email { get; set; }
    //        public int id { get; set; }
    //        public int integration { get; set; }
    //        public object metadata { get; set; }
    //        public string name { get; set; }
    //        public string recipient_code { get; set; }
    //        public string type { get; set; }
    //        public bool is_deleted { get; set; }
    //        public Details details { get; set; }
    //        public DateTime created_at { get; set; }
    //        public DateTime updated_at { get; set; }
    //    }

    //}

    //public class PaystackSubscriptionDataModel
    //{
    //    public string domain { get; set; }
    //    public string status { get; set; }
    //    public string subscription_code { get; set; }
    //    public string email_token { get; set; }
    //    public int amount { get; set; }
    //    public string cron_expression { get; set; }
    //    public DateTime next_payment_date { get; set; }
    //    public object open_invoice { get; set; }
    //    public DateTime createdAt { get; set; }
    //    public Plan plan { get; set; }
    //    public Authorization authorization { get; set; }
    //    public Customer customer { get; set; }
    //    public DateTime created_at { get; set; }

    //    public class Authorization
    //    {
    //        public string authorization_code { get; set; }
    //        public string bin { get; set; }
    //        public string last4 { get; set; }
    //        public string exp_month { get; set; }
    //        public string exp_year { get; set; }
    //        public string card_type { get; set; }
    //        public string bank { get; set; }
    //        public string country_code { get; set; }
    //        public string brand { get; set; }
    //        public string account_name { get; set; }
    //    }

    //    public class Customer
    //    {
    //        public string first_name { get; set; }
    //        public string last_name { get; set; }
    //        public string email { get; set; }
    //        public string customer_code { get; set; }
    //        public string phone { get; set; }
    //        public object metadata { get; set; }
    //        public string risk_action { get; set; }
    //    }

    //    public class Plan
    //    {
    //        public int id { get; set; }
    //        public string name { get; set; }
    //        public string plan_code { get; set; }
    //        public object description { get; set; }
    //        public int amount { get; set; }
    //        public string interval { get; set; }
    //        public bool send_invoices { get; set; }
    //        public bool send_sms { get; set; }
    //        public string currency { get; set; }
    //    }

    //}
}
