import React from "react";
import Product from "./Product";
import axios from "axios";

export default class ProductList extends React.Component {
  state = {
    products: [],
  };

  componentDidMount() {
    axios.get(`http://localhost:8080/api/v1/products`).then((res) => {
      const products = res.data;
      this.setState({ products });
    });
  }

  render() {
    return (
      <ul>
        {this.state.products.map((product: any) => (
          <li>
            <Product
              name={product.name}
              description={product.description}
              price={product.price}
            />
          </li>
        ))}
      </ul>
    );
  }
}
