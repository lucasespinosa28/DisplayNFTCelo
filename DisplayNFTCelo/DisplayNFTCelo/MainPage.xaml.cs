using System;
using Nethereum.Web3;
using System.Net.Http.Json;
using Xamarin.Forms;
using System.Net.Http;


namespace DisplayNFTCelo
{
    public partial class MainPage : ContentPage
    {
        private string url = "https://forno.celo.org";
        private string ABI = @"[{""type"":""function"",""stateMutability"":""view"",""outputs"":[{""type"":""string"",""name"":"""",""internalType"":""string""}],""name"":""tokenURI"",""inputs"":[{""type"":""uint256"",""name"":""tokenId"",""internalType"":""uint256""}]}]";
        public MainPage()
        {
            InitializeComponent();
        }
        string ipfsHost(string url)
        {
            //Check if are an IPFS address
            if (url.Contains("ipfs://"))
            {
                //Add an IPFS host 
                return url.Replace("ipfs://", "https://ipfs.io/ipfs/");
            }
            //Return an not IPFS address
            return url;
        }
        class Metadata
        {
            public string name { get; set; }
            public string description { get; set; }
            public string external_url { get; set; }
            public string image { get; set; }
            public Attribute[] attributes { get; set; }
            public class Attribute
            {
                public string trait_type { get; set; }
                public string value { get; set; }
            }

        }
        async void queryAsyncMetadata(string uri)
        {
            // create a new connection
            var client = new HttpClient();
            //get the metadata in format json em convert it's for a type metadata
            var response = await client.GetFromJsonAsync<Metadata>(ipfsHost(uri));
            //Display the name in a <label> 
            Name.Text = response.name;
            //If image only have an ipfs address the method ipfsHost will add a host, Display the image in a <Image>
            NFTImage.Source = ipfsHost(response.image);
            //Display the description in a <label> 
            Description.Text = response.description;

        }
        async void queryNFTContract(int id)
        {
            //connect to a celo node
            var web3 = new Web3(url);
            //read a smart contract
            var contract = web3.Eth.GetContract(ABI, Address.Text);
            //Create a function to get a NFT metadata url
            var tokenURIFunction = contract.GetFunction("tokenURI");
            //Call the function to get a NFT metadata url
            var metadata = await tokenURIFunction.CallAsync<string>(id);
            //Pass the NFT metadata URL and display NFT imagen and information in the app
            queryAsyncMetadata(metadata);
        }
        void getNFT(object sender, EventArgs e)
        {
            //Read the input that have a nft id to query
            var id = int.Parse(NFTid.Text);
            //Pass the id to read the contract and display the NFT.
            queryNFTContract(id);
        }

        Random rnd = new Random();
        void getRandomNFT(object sender, EventArgs e)
        {
            //Query an NFT with random ID between 1 or 100
            queryNFTContract(rnd.Next(1, 100));
        }
    }
}
