﻿using System.Linq;
using System.Security.Cryptography;
using Asn1;

namespace EasySsl {
    public class RsaPublicKey : X509PublicKey {

        public RsaPublicKey(RSAParameters parameters) {
            Exponent = parameters.Exponent;
            Modulus = parameters.Modulus;
        }

        public RsaPublicKey(Asn1BitString valueNode) {
            var value = Asn1Node.ReadNode(valueNode.Data);
            Modulus = GetRsaData((Asn1Integer)value.Nodes[0]);
            Exponent = GetRsaData((Asn1Integer)value.Nodes[1]);
        }

        public byte[] Exponent { get; set; }

        public byte[] Modulus { get; set; }

        public override AsymmetricAlgorithm CreateAsymmetricAlgorithm() {
            var args = ToRsaParameters();
            var rsa = new RSACryptoServiceProvider();
            rsa.ImportParameters(args);
            return rsa;
        }

        public override byte[] GenerateIdentifier() {
            var data = Modulus.Concat(Exponent).ToArray();
            var sha = SHA1.Create();
            var hash = sha.ComputeHash(data);
            hash[0] &= 0x7f;
            return hash;
        }

        protected override string PemName
        {
            get {
                return "RSA PUBLIC KEY"; 
            }
        }

        public override X509AlgorithmIdentifier Algorithm
        {
            get { return X509AlgorithmIdentifier.RsaEncryption; }
        }

        //https://tools.ietf.org/html/rfc3447#appendix-A.1.1
        public override Asn1Node ToAsn1() {
            return new Asn1Sequence {
                Nodes = {
                    GetAsn1Integer(Modulus),
                    GetAsn1Integer(Exponent)
                }
            };
        }

        public RSAParameters ToRsaParameters() {
            return new RSAParameters {
                Modulus = Modulus,
                Exponent = Exponent
            };
        }

        private static Asn1Integer GetAsn1Integer(byte[] data) {
            if ((data[0] & 0x80) == 0) return new Asn1Integer(data);
            return new Asn1Integer(new byte[] { 0 }.Concat(data).ToArray());
        }

        private static byte[] GetRsaData(Asn1Integer node) {
            if (node.Value.Length == 257) {
                return node.Value.Skip(1).ToArray();
            }
            if (node.Value.Length == 129) {
                return node.Value.Skip(1).ToArray();
            }
            return node.Value;
        }

    }
}
