using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data structure for holding links
public class EdgeLink{
    
    // Node to Node
    Node a;
    Node b;

    public EdgeLink(Node a, Node b) {
        this.a = a;
        this.b = b;
    }

    // Return IDS
    public Node getA() {
        return a;
    }

    public Node getB() {
        return b;
    }

    // Return the HashCode to avoid duplicates
    public override int GetHashCode() {     
        int x = (int)a.position.x;
        int y = (int)b.position.x;
        int z = (int)a.position.z;
        int x_d = (int)((a.position.x - (int)a.position.x)*100);
        int y_d = (int)((b.position.x - (int)b.position.x)*100);
        int z_d = (int)((b.position.y - (int)b.position.y)*100);
        int a_d = (int)((a.position.y - (int)a.position.y)*100);
        int a_z = (int)((a.position.z - (int)a.position.x)*100);
        int a_y = (int)((a.position.y - (int)a.position.x)*100);
        int b_y = (int)((b.position.y - (int)b.position.x)*100);
        //int hashcode = (x + y + z + a_z + a_y) + (x_d + y_d + z_d);
        int hashcode = a.GetHashCode() + b.GetHashCode() + (x + y);
        return hashcode;
    }
    // Stops duplicates from being added
    public override bool Equals(object obj) {
        return obj.GetHashCode() == this.GetHashCode();
    }
}
