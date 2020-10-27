import React, { useContext } from "react";
import Product from "./Product";
import axios from "axios";
import { isContext } from "vm";
import GetProducts from "../../api/GetProducts";

const ProductList = () => {
  const products = GetProducts();
  return (
    <>
      <p>asdf</p>
      {products.map((p) => (
        <p>{p.name}</p>
      ))}
    </>
  );
};

export default ProductList;
