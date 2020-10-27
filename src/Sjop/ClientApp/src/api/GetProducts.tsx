import React from "react";
import axios from "axios";

function GetProducts(): any {
  axios.get(`http://localhost:8080/api/v1/products`).then((res) => {
    const products = res.data;
    return products;
  });
}

export default GetProducts;
