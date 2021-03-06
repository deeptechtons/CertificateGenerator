﻿using System;
using System.Text;
using Asn1;

namespace EasySsl
{
    public class PrivateKeyInfo
    {

        public PrivateKeyInfo()
        {
            Version = new Asn1Integer(0);
        }

        public Asn1Integer Version { get; set; }

        public X509AlgorithmIdentifier PrivateKeyAlgorithmIdentifier { get; set; }

        public byte[] PrivateKey { get; set; }

        //https://tools.ietf.org/html/rfc5208#section-5
        public Asn1Node ToAsn1()
        {
            return new Asn1Sequence
            {
                Nodes = {
                    Version,
                    PrivateKeyAlgorithmIdentifier.ToAsn1(),
                    new Asn1OctetString(PrivateKey)
                }
            };
        }

        public string ToPem()
        {
            var sb = new StringBuilder();
            sb.AppendLine("-----BEGIN PRIVATE KEY-----");
            var data = ToAsn1();
            var bytes = data.GetBytes();
            sb.AppendLine(Convert.ToBase64String(bytes, Base64FormattingOptions.InsertLineBreaks));
            sb.AppendLine("-----END PRIVATE KEY-----");
            return sb.ToString();
        }
    }
}
