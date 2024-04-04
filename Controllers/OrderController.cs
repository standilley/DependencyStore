using Dapper;
using DependencyStore.Models;
using DependencyStore.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RestSharp;

namespace DependencyStore.Controllers;

public class OrderController : ControllerBase
{
    private readonly ICustomerRepository _customerRepository;
    public OrderController(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }
    [Route("v1/orders")]
    [HttpPost]
    public async Task<IActionResult> Place(
        string customerId,
        string zipCode,
        string promoCode,
        int[] products)
    {
        // #1 - Recupera o cliente
        var customer = await _customerRepository.GetByIdAsync(customerId);
        if (customer == null)
            return NotFound();

        // #2 - Calcula o frete
        decimal deliveryFee = 0;
        var client = new RestClient("https://consultafrete.io/cep/");
        var request = new RestRequest()
            .AddJsonBody(new
            {
                zipCode
            });
        deliveryFee = await client.PostAsync<decimal>(request, new CancellationToken());
        // Nunca é menos que R$ 5,00
        if (deliveryFee < 5)
            deliveryFee = 5;

        // #3 - Calcula o total dos produtos
        decimal subTotal = 0;
        const string getProductQuery = "SELECT [Id], [Name], [Price] FROM PRODUCT WHERE ID=@id";
        for (var p = 0; p < products.Length; p++)
        {
            Product product;
            await using (var conn = new SqlConnection("CONN_STRING"))
                product = await conn.QueryFirstAsync<Product>(getProductQuery, new { Id = p });

            subTotal += product.Price;
        }

        // #4 - Aplica o cupom de desconto
        decimal discount = 0;
        await using (var conn = new SqlConnection("CONN_STRING"))
        {
            const string query = "SELECT * FROM PROMO_CODES WHERE CODE=@code";
            var promo = await conn.QueryFirstAsync<PromoCode>(query, new { code = promoCode });
            if (promo.ExpireDate > DateTime.Now)
                discount = promo.Value;
        }

        // #5 - Gera o pedido
        var order = new Order();
        order.Code = Guid.NewGuid().ToString().ToUpper().Substring(0, 8);
        order.Date = DateTime.Now;
        order.DeliveryFee = deliveryFee;
        order.Discount = discount;
        order.Products = products;
        order.SubTotal = subTotal;

        // #6 - Calcula o total
        order.Total = subTotal - discount + deliveryFee;

        // #7 - Retorna
        return Ok(new
        {
            Message = $"Pedido {order.Code} gerado com sucesso!"
        });
    }
}